# OmidBankSepah

## نگاه من به معماری و CoreLibrary

`Taday.Corelibrary` را خودم نوشته‌ام و طی چند سال، با استفاده از آن در پروژه‌ها و اصلاح تدریجی‌اش، به بلوغ رسانده‌ام. پایه‌هایی مثل `AggregateRoot`، `BaseEntity`، `BaseDbContext`، `CoreExeptionHandlerMiddleware`، `RabbitMqBaseService`، `BaseRepository`، `BaseSpecification` و `CriteriaSpecification` داخل همین کتابخانه‌اند. من اساس سرویس‌های این پروژه را روی این Core گذاشته‌ام؛ برای همین در ارائه‌ی کدچلنج، اول توضیح می‌دهم این پایه‌ها چه مسئولیتی دارند و بعد سراغ خود سرویس‌ها می‌روم.

### دامنه، Aggregate و اعتبارسنجی

- من قواعد کسب‌وکار را در Domain نگه داشته‌ام. Aggregate Rootها از `AggregateRoot` و موجودیت‌های داخل Aggregate از `BaseEntity` مشتق می‌شوند؛ تغییر وضعیت را از رفتارهای خود دامنه انجام می‌دهم، نه با setterهای آزاد. `BaseEntity` متد `ImmediateFillId()` را برای زمانی که سازنده باید شناسه را همان ابتدا بسازد فراهم می‌کند؛ این کار خودکار در `SaveChanges` انجام نمی‌شود.
- من اعتبارسنجی پیش از ذخیره را در `BaseDbContext` متمرکز کرده‌ام: ChangeTracker بررسی می‌شود و `IValidatableObject`های Added/Modified اعتبارسنجی می‌شوند. اگر `ValidationResult` داشته باشیم، ذخیره متوقف و خطای دامنه تولید می‌شود؛ خطایی که از مسیر ذخیره‌سازی عبور کند را Middleware مرکزی به پاسخ استاندارد API تبدیل می‌کند.
- من Domain Eventها را روی Aggregate Root نگه می‌دارم. پس از ذخیره‌ی موفق، `BaseDbContext` رویدادهای Aggregate Rootهای Trackشده را منتشر و بعد پاک می‌کند.
- یک نکته‌ی فعلی Core را هم شفاف می‌گویم: overload `SaveChangesAsync(CancellationToken)` استثناها را می‌گیرد و `0` برمی‌گرداند. پس استثنای این مسیر به Middleware نمی‌رسد؛ اگر بخواهم Middleware آن خطا را به کاربر برگرداند، باید exception از مسیر ذخیره‌سازی propagate شود.

### Specification و Repository

من فیلترها، مرتب‌سازی، صفحه‌بندی و Includeهای هر درخواست داده را در Specificationهای Domain تعریف می‌کنم؛ Repository زیرساختی همان Specification را روی query اعمال می‌کند و نتیجه را به‌شکل مجموعه (مثلاً `List`) یا توالی برمی‌گرداند. `BaseSpecification` پایه‌ی این الگوست. برای شرط‌ها هم `CriteriaSpecification` را گذاشته‌ام تا بتوانم آن‌ها را با `And`، `Or` و `Not` ترکیب کنم؛ `MergedSpecification` هم چند Specification را کنار هم ارزیابی می‌کند.

من برای هر Aggregate Root قرارداد Repository دامنه‌ای تعریف می‌کنم و Repository عمومی را مستقیم به سرویس‌ها نمی‌دهم. `BaseRepository` در Core هم abstract است و نوعی که می‌گیرد باید `IAggregateRoot` باشد. به این ترتیب، در مسیر معمول برنامه DbContext یا Repository عمومی را مستقیم Inject نمی‌کنم؛ پیاده‌سازی EF Core و Context در Infrastructure است و Domain به جزئیات دیتابیس وابسته نیست.

### Application، قراردادها و خطاها

من در Application جریان Use Case را هماهنگ می‌کنم و Domain Serviceها، Repositoryها و سرویس‌های بیرونی را به هم وصل می‌کنم؛ قانون اصلی کسب‌وکار را آنجا کپی نمی‌کنم و در Domain Service یا Aggregate نگه می‌دارم.

برای اینکه Entity خام از Domain بیرون نرود، من مرز ورود و خروج را با Contractهای `Req` و `Res` مشخص کرده‌ام:

- وقتی DTO یا پیام ورودی قرار است وارد منطق Domain شود، آن را به Contract درخواست دامنه (`Req`) تبدیل می‌کنم؛ Domain Service به‌جای DTO مربوط به HTTP یا gRPC، همین ورودی دامنه را دریافت می‌کند.
- وقتی نتیجه از Domain بیرون می‌آید، Entity را مستقیم به DTO تبدیل یا serialize نمی‌کنم؛ آن را به Contract پاسخ دامنه (`Res`) تبدیل می‌کنم و لایه‌ی بیرونی در صورت نیاز `Res` را به پاسخ HTTP یا gRPC خودش map می‌کند.

پس هدف Contract برای من صرفاً داشتن چند کلاس Request/Response نیست: Entity هیچ‌وقت به‌صورت خام از Domain خارج نمی‌شود و Domain هم به شکل DTOهای مخصوص Presentation آلوده نمی‌شود. اینترفیس‌های Application را در پوشه‌ی `Interfaces` گذاشته‌ام و برای هر Use Case، Command و Handler را کنار هم در پوشه‌ی همان Use Case نگه می‌دارم.

برای خطاهای دامنه و ورودی، Assertهای اختصاصی گذاشته‌ام تا خطا با نوع و اطلاعات مناسب تولید شود. Middleware هر سرویس از `CoreExeptionHandlerMiddleware` ارث می‌برد و exceptionهایی را که تا HTTP pipeline برسند به پاسخ استاندارد تبدیل می‌کند؛ بنابراین خطا نباید در لایه‌ی پایین‌تر بلعیده شود یا به شکل موفقیت برگردد.

## ارتباط بین Microserviceها و RabbitMQ

من در این راهکار دو نوع ارتباط را با دو هدف متفاوت گذاشته‌ام:

- **gRPC برای درخواست هم‌زمان:** در جریان ساخت سفارش، `Order` از طریق `MasterDataProductAvailabilityGrpcProxy` به سرویس gRPC مربوط به `MasterData` درخواست رزرو موجودی می‌فرستد. `ProductAvailabilityGrpcService` در MasterData درخواست را به Contractهای Domain تبدیل می‌کند، رزرو را انجام می‌دهد و نتیجه‌ی `IsReserved` و اقلام ناموجود را برمی‌گرداند؛ Order بر اساس همان پاسخ سفارش را `Confirmed` یا `Rejected` می‌کند. MasterData متدهای gRPC دیگری برای افزایش و خواندن موجودی هم دارد، اما در جریان سفارش فعلی، Order متد رزرو را صدا می‌زند.
- **RabbitMQ برای خبررسانی غیرهم‌زمان:** پس از افزایش موجودی یا رزرو موفق، MasterData رویداد `ProductInventoryChangedMessage` را منتشر می‌کند. Order، MasterData، Wallet و Identity هرکدام صف اختصاصی دارند و پیام را از طریق MediatR به Command Handler خودشان می‌رسانند. Handlerها فعلاً TODO اتصال به Notification Service هستند؛ منطق ارسال اعلان هنوز پیاده‌سازی نشده است.

### پیکربندی Exchange و Queue

تنظیمات اتصال و نام صف‌ها در بخش `RabbitMq` هر سرویس قرار دارند. نمونه‌ی زیر برای صف Order است؛ نام کاربری و رمز عبور را از User Secrets یا متغیر محیطی بدهید و از قرار دادن اعتبارنامه‌ی واقعی در مخزن خودداری کنید:

```json
{
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "<rabbit-user>",
    "Password": "<rabbit-password>",
    "ConnectionName": "Order Service",
    "Ssl": false,
    "PrefetchCount": 5,
    "AutomaticRecoveryEnabled": true,
    "RetryConnectionIntervalInSecond": 2,
    "ProductInventoryQueue": "Order.ProductInventory",
    "SubscribingQueues": {
      "Order.ProductInventory": {
        "Exchange": "ProductInventory",
        "AutoAck": false,
        "IsBinding": true,
        "ServiceName": "Order",
        "JitterDelayFrom": 100,
        "JitterDelayTo": 500,
        "JitterMaxRetry": 5
      }
    }
  }
}
```

`ProductInventoryQueue` باید با یکی از کلیدهای `SubscribingQueues` برابر باشد؛ هر سرویس از نام صف متفاوتی استفاده می‌کند، اما همگی به Exchange مشترک `ProductInventory` متصل می‌شوند. Exchange از نوع Fanout است، پس پیام به تمام صف‌های bound می‌رسد و `RoutingKey` در مسیریابی Fanout نقشی ندارد. `IsBinding` اتصال صف به Exchange را کنترل می‌کند. `AddHostedService` باعث می‌شود Host هنگام بالا آمدن سرویس، consumer را راه‌اندازی کند.

در `RabbitMqBaseService` که برای این پروژه استفاده کرده‌ام، صف و Exchange به‌صورت Durable ساخته می‌شوند. با `AutoAck: false`، پیام بعد از پردازش موفق ACK می‌شود؛ خطاها تا `JitterMaxRetry` بار با تأخیر jitterدار دوباره امتحان می‌شوند و بعد با `requeue: false` رد می‌شوند. چون Dead Letter Exchange/Queue نداریم، پیام ناموفق ممکن است حذف شود. Publisher هم فعلاً پیام را persistent علامت نمی‌زند و Publisher Confirm یا Outbox ندارد. برای همین من ادعای «عدم گم‌شدن پیام» یا تضمین ۹۹٪ نمی‌کنم؛ Durable بودن صف و Exchange به‌تنهایی برای چنین تضمینی کافی نیست. اگر بخواهم این تضمین را اضافه کنم، باید Outbox، پیام persistent، Publisher Confirm و Dead Letter Queue را پیاده‌سازی و آزمون کنم.

## پیش‌نیازها

- نصب .NET 9 SDK
- دسترسی به SQL Server
- در دسترس بودن Redis در `localhost:6379` یا تنظیم مقدار `Redis:ConnectionString`
- در دسترس بودن RabbitMQ در `localhost:5672` یا تنظیم مقادیر بخش `RabbitMq`
- دسترسی به فید خصوصی GitHub Packages برای بسته‌ی `Taday.Corelibrary`

پیش از بازیابی بسته‌ها، فید NuGet زیر را اضافه کنید:

```powershell
dotnet nuget add source --username samiyariceboy --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github https://nuget.pkg.github.com/samiyariceboy/index.json
```

به‌جای `YOUR_GITHUB_TOKEN` توکنی با دسترسی خواندن بسته‌ها (`read:packages`) قرار دهید.

> **امنیت:** گزینه‌ی `--store-password-in-clear-text` اعتبارنامه را بدون رمزنگاری در تنظیمات NuGet ذخیره می‌کند. توکن واقعی را در مخزن، لاگ یا تصویر صفحه قرار ندهید و از توکنی با حداقل دسترسی لازم استفاده کنید.

## معماری و جریان سفارش

سرویس Order مالک اطلاعات و چرخه‌ی عمر سفارش است و سرویس MasterData مالک اطلاعات محصول و موجودی قابل‌دسترس آن. هر سرویس داده‌های خود را در پایگاه‌داده‌ی SQL Server نگه می‌دارد. ارتباط داخلی Order با MasterData از طریق gRPC و APIهای قابل‌استفاده‌ی بیرونی از طریق HTTP انجام می‌شوند.

جریان ثبت سفارش:

1. شناسه‌ی مشتری، کلید idempotency، شناسه‌های محصول، قیمت‌ها و تعدادهای مثبت بررسی می‌شوند؛ سپس سفارش با وضعیت `Pending` ذخیره می‌شود.
2. سرویس Order با شناسه‌ی سفارش و تعداد هر محصول، درخواست gRPC برای رزرو موجودی قابل‌دسترس می‌فرستد.
3. سرویس MasterData ابتدا همه‌ی اقلام را بررسی می‌کند. فقط در صورت کافی‌بودن موجودی تمام اقلام، کاهش تعدادها و ذخیره‌ی نتیجه‌ی رزرو در یک تراکنش پایگاه‌داده انجام می‌شود. در صورت ناکافی‌بودن حتی یک قلم، هیچ تعدادی تغییر نمی‌کند.
4. سرویس Order با توجه به نتیجه‌ی ذخیره‌شده، وضعیت سفارش را به `Confirmed` یا `Rejected` تغییر می‌دهد.

برای جلوگیری از پردازش تکراری، شناسه‌ی سفارش در MasterData یکتا است و در Order روی کلید idempotency ایندکس یکتا وجود دارد. تکرار همان درخواست با همان کلید، نتیجه‌ی قبلی را برمی‌گرداند؛ استفاده از همان کلید برای محتوای متفاوت رد می‌شود. RedLock از رقابت هم‌زمان می‌کاهد و ایندکس‌های یکتا و RowVersion در SQL Server حفاظت تکمیلی هستند. اگر قفل دریافت نشود، عملیات ادامه پیدا نمی‌کند.

جریان فعلی رزرو همچنان به‌صورت هم‌زمان با gRPC اجرا می‌شود. پس از افزایش موفق موجودی یا رزرو موفق، MasterData یک رویداد تغییر موجودی را به Exchange از نوع Fanout با نام `ProductInventory` منتشر می‌کند. سرویس‌های Order، MasterData، Wallet و Identity هر کدام صف بادوام جداگانه‌ای دارند؛ مصرف‌کننده پیام را با MediatR به Command Handler همان سرویس می‌فرستد. Handlerها فعلاً TODO برای فراخوانی Notification Service دارند. انتشار پیام best-effort است: خطا ثبت می‌شود و باعث شکست جریان فعلی موجودی یا سفارش نمی‌شود؛ در حال حاضر Outbox برای تضمین انتشار پس از قطعی RabbitMQ وجود ندارد.

در صورت قطع ارتباط gRPC یا در دسترس نبودن پایگاه‌داده‌ی Order، سفارش می‌تواند `Pending` بماند؛ همان درخواست را با همان کلید idempotency دوباره ارسال کنید. اگر موجودی کم شده باشد ولی وضعیت نهایی سفارش ذخیره نشده باشد، تلاش مجدد نتیجه‌ی ذخیره‌شده‌ی MasterData را بازیابی می‌کند. هنوز فرایند آزادسازی جبرانی برای سفارش‌هایی که هیچ‌وقت دوباره ارسال نمی‌شوند وجود ندارد.

فرض‌ها و محدودیت‌ها:

- محصول باید از قبل در MasterData وجود داشته باشد. رکورد موجودی با اولین افزایش موجودی قابل‌دسترس ساخته می‌شود؛ در حال حاضر API ساخت محصول در این مخزن ارائه نشده است.
- شناسه‌ی مشتری از نظر خالی‌نبودن بررسی می‌شود، اما با سرویس جداگانه‌ای تطبیق داده نمی‌شود.
- تنظیم پیش‌فرض RedLock به یک Redis محلی متصل است و برای دسترس‌پذیری Production quorum محسوب نمی‌شود؛ برای محیط واقعی، توپولوژی Redis مناسب در کنار قیود و کنترل هم‌زمانی SQL تنظیم شود.

## اجرا در محیط محلی

هر دو Migration پایگاه‌داده را اجرا کنید. رشته‌های اتصال را متناسب با محیط خود از طریق User Secrets یا متغیر محیطی تنظیم کنید:

```powershell
dotnet ef database update --project src\MasterData\OmidBankSepah.MasterData.Infrastructure\OmidBankSepah.MasterData.Infrastructure.csproj --startup-project src\MasterData\OmidBankSepah.MasterData.Presentation\OmidBankSepah.MasterData.Presentation.csproj
dotnet ef database update --project src\Order\OmidBankSepah.Order.Infrastructure\OmidBankSepah.Order.Infrastructure.csproj --startup-project src\Order\OmidBankSepah.Order.Presentation\OmidBankSepah.Order.Presentation.csproj
```

Redis و RabbitMQ را اجرا کنید و هر سرویس را در یک ترمینال جداگانه بالا بیاورید:

```powershell
dotnet run --project src\MasterData\OmidBankSepah.MasterData.Grpc\OmidBankSepah.MasterData.Grpc.csproj
dotnet run --project src\MasterData\OmidBankSepah.MasterData.Presentation\OmidBankSepah.MasterData.Presentation.csproj
dotnet run --project src\Order\OmidBankSepah.Order.Presentation\OmidBankSepah.Order.Presentation.csproj
dotnet run --project src\Wallet\OmidBankSepah.Wallet.Presentation\OmidBankSepah.Wallet.Presentation.csproj
dotnet run --project src\Identity\OmidBankSepah.Identity.Presentation\OmidBankSepah.Identity.Presentation.csproj
```

نشانی‌های پیش‌فرض:

- gRPC سرویس MasterData: `http://localhost:5085`
- API HTTP سرویس MasterData: `http://localhost:5154`
- API HTTP سرویس Order: `http://localhost:5086`

رشته‌های اتصال موجود در تنظیمات برای توسعه‌ی محلی هستند؛ برای محیط‌های دیگر آن‌ها را بیرون از مخزن تنظیم کنید.

## نمونه‌ی API

افزایش موجودی قابل‌دسترس محصولی که از قبل ثبت شده است:

```http
POST http://localhost:5154/api/product-availability/{productId}/increase
Content-Type: application/json

{ "quantity": 10 }
```

برای مشاهده‌ی موجودی قابل‌دسترس از این نشانی استفاده کنید:
`GET http://localhost:5154/api/product-availability/{productId}`

ایجاد سفارش؛ هنگام تلاش مجدد برای همان درخواست، `idempotencyKey` را تغییر ندهید:

```http
POST http://localhost:5086/api/orders
Content-Type: application/json

{
  "customerId": "11111111-1111-1111-1111-111111111111",
  "idempotencyKey": "checkout-2026-001",
  "items": [
    {
      "productId": "22222222-2222-2222-2222-222222222222",
      "price": 125000,
      "quantity": 2
    }
  ]
}
```

برای مشاهده‌ی وضعیت و جزئیات سفارش از `GET http://localhost:5086/api/orders/{orderId}` استفاده کنید. در صورت قطعی موقت سرویس موجودی، API کد `503` برمی‌گرداند و سفارش برای تلاش مجدد با همان کلید در وضعیت `Pending` باقی می‌ماند.

## آزمون‌ها

برای اجرای آزمون‌های واحد و یکپارچه:

```powershell
dotnet test OmidBankSepah.Order.Presentation.slnx
```

آزمون یکپارچه، ارتباط gRPC را با سرور HTTP/2 درون‌حافظه‌ای بررسی می‌کند. آزمون‌های واحد منطق دامنه‌ی سفارش و موجودی را پوشش می‌دهند؛ اجرای آن‌ها به SQL Server یا Redis واقعی نیاز ندارد.

مجموعه‌ی سناریوهای دستی Postman در [POSTMAN/OmidBankSepah.postman_collection.json](./POSTMAN/OmidBankSepah.postman_collection.json) قرار دارد. پس از بالا آوردن سرویس‌های موردنیاز، فایل را در Postman import و سناریوهای موجودی، رزرو و سفارش را اجرا کنید.
