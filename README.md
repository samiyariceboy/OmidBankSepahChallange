# OmidBankSepah

## مبانی معماری و CoreLibrary

`Taday.Corelibrary` توسط من نوشته شده و طی چند سال، با استفاده در پروژه‌های مختلف و اصلاح تدریجی، به بلوغ رسیده است. این کتابخانه پایه‌هایی مانند `AggregateRoot`، `BaseEntity`، `BaseDbContext`، `CoreExeptionHandlerMiddleware`، `RabbitMqBaseService`، `BaseRepository`، `BaseSpecification` و `CriteriaSpecification` را فراهم می‌کند و مبنای مشترک سرویس‌های این پروژه است. در ارائه‌ی کدچلنج، ابتدا این پایه‌ها و نقش آن‌ها توضیح داده می‌شوند.

### دامنه، Aggregate و اعتبارسنجی

- قواعد کسب‌وکار در Domain قرار دارند. Aggregate Rootها از `AggregateRoot` و موجودیت‌های داخل Aggregate از `BaseEntity` مشتق می‌شوند؛ تغییر وضعیت از طریق رفتارهای دامنه انجام می‌شود، نه با setterهای آزاد. `BaseEntity` متد `ImmediateFillId()` را برای تولید شناسه‌ی فوری در اختیار سازنده‌ی موجودیت قرار می‌دهد؛ تولید شناسه هنگام `SaveChanges` به‌صورت خودکار انجام نمی‌شود.
- `BaseDbContext` پیش از نوشتن، ChangeTracker را بررسی می‌کند و `IValidatableObject`های Added/Modified را اعتبارسنجی می‌کند. خطاهای `ValidationResult` جمع‌آوری می‌شوند و در صورت وجود، مسیر ذخیره‌سازی متوقف و خطای دامنه تولید می‌شود. در مسیرهایی که exception را حفظ و منتشر می‌کنند، Middleware مرکزی خطا را به پاسخ استاندارد API تبدیل می‌کند.
- Aggregate Rootها نگهدارنده‌ی Domain Eventها هستند؛ `BaseDbContext` پس از ذخیره‌ی موفق، رویدادهای موجود در Aggregate Rootهای Trackشده را منتشر و سپس پاک می‌کند.
- نکته‌ی پیاده‌سازی فعلی Core: overload `SaveChangesAsync(CancellationToken)` استثناها را می‌گیرد و `0` برمی‌گرداند. بنابراین استثناهای این مسیر به Middleware نمی‌رسند؛ برای نمایش این خطاها توسط Middleware، باید exception در مسیر ذخیره‌سازی propagate شود.

### Specification و Repository

در Domain، Specificationها محل تعریف فیلترها، مرتب‌سازی، صفحه‌بندی و Includeهای موردنیاز هستند؛ Repository زیرساختی آن‌ها را روی query اعمال می‌کند و خروجی را به‌شکل مجموعه‌ی مادی‌شده (مانند `List`) یا توالی برمی‌گرداند. `BaseSpecification` امکان استفاده و ترکیب Specificationها را فراهم می‌کند و `CriteriaSpecification` قابلیت ترکیب شرط‌ها با `And`، `Or` و `Not` را دارد. `MergedSpecification` نیز برای ارزیابی چند Specification در کنار هم وجود دارد.

طبق قاعده‌ی DDD این راهکار، Repository اختصاصی برای Aggregate Root تعریف می‌شود. `BaseRepository` در Core نیز نوع موجودیت را به `IAggregateRoot` محدود کرده و به‌صورت abstract ارائه شده است؛ سرویس‌ها از Repositoryهای دامنه‌ای استفاده می‌کنند، نه از پیاده‌سازی عمومی آن. مرز پروژه‌ها Domain را از EF Core و جزئیات پایگاه‌داده جدا نگه می‌دارد و پیاده‌سازی Repository و Context در Infrastructure قرار دارد.

### Application، قراردادها و خطاها

Application مسئول هماهنگی Use Caseها، مدیریت جریان کار و فراخوانی Domain Serviceها و وابستگی‌های بیرونی است؛ قواعد اصلی کسب‌وکار در Domain Service و Aggregateها نگهداری می‌شوند.

Contractها مرز ورود و خروج داده به Domain را مشخص می‌کنند:

- وقتی DTO یا پیام ورودی وارد منطق Domain می‌شود، ابتدا به Contract درخواست دامنه (`Req`) تبدیل می‌شود؛ Domain Service به‌جای DTO مربوط به HTTP یا gRPC، همین ورودی دامنه را دریافت می‌کند.
- وقتی نتیجه از Domain خارج می‌شود، Entity مستقیماً به DTO تبدیل یا serialize نمی‌شود؛ به Contract پاسخ دامنه (`Res`) تبدیل می‌شود و لایه‌ی بیرونی در صورت نیاز `Res` را به پاسخ HTTP یا gRPC خودش map می‌کند.

برای مثال، درخواست رزرو gRPC پیش از ورود به منطق Domain به Contractهای `Req` دامنه تبدیل می‌شود؛ نتیجه‌ی موجودی نیز به `Res` تبدیل و سپس به پاسخ gRPC نگاشت می‌شود. به این ترتیب Entity مستقیماً و به‌شکل خام از Domain خارج نمی‌شود و Domain به DTOهای مخصوص Presentation وابسته نیست. اینترفیس‌های Application در پوشه‌ی `Interfaces` نگهداری می‌شوند و هر Use Case، Command و Handler خود را در پوشه‌ی مربوط به همان Use Case دارد.

برای خطاهای دامنه و ورودی از Assertهای اختصاصی استفاده می‌شود تا خطاها با نوع و داده‌ی مناسب تولید شوند. Middlewareهای سرویس‌ها از `CoreExeptionHandlerMiddleware` ارث می‌برند و exceptionهای قابل‌انتشار را به ساختار پاسخ استاندارد تبدیل می‌کنند. این قرارداد زمانی برقرار است که لایه‌ی پایین‌تر exception را نبلعد یا به مقدار موفقیت‌نما تبدیل نکند.

## ارتباط بین Microserviceها و RabbitMQ

در این راهکار دو نوع ارتباط برای دو هدف متفاوت استفاده می‌شود:

- **gRPC برای درخواست هم‌زمان:** در جریان ساخت سفارش، سرویس Order از طریق `MasterDataProductAvailabilityGrpcProxy` به سرویس gRPC در MasterData درخواست رزرو موجودی می‌فرستد. `ProductAvailabilityGrpcService` درخواست را به Contractهای Domain تبدیل می‌کند، رزرو را انجام می‌دهد و نتیجه‌ی `IsReserved` و اقلام ناموجود را برمی‌گرداند؛ Order بر اساس پاسخ، سفارش را `Confirmed` یا `Rejected` می‌کند. سرویس gRPC در MasterData متدهایی برای افزایش و خواندن موجودی هم دارد، اما در جریان فعلی سفارش، Order متد رزرو را فراخوانی می‌کند.
- **RabbitMQ برای خبررسانی غیرهم‌زمان:** پس از افزایش موجودی یا رزرو موفق، MasterData رویداد `ProductInventoryChangedMessage` را منتشر می‌کند. سرویس‌های Order، MasterData، Wallet و Identity هرکدام صف اختصاصی دارند و پیام را از طریق MediatR به Command Handler خود می‌فرستند. Handlerها فعلاً TODO اتصال به Notification Service دارند و منطق ارسال اعلان هنوز پیاده‌سازی نشده است.

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

`RabbitMqBaseService` صف و Exchange را به‌صورت Durable تعریف می‌کند. با `AutoAck: false`، پس از پردازش موفق پیام ACK می‌شود؛ خطاها تا `JitterMaxRetry` بار با تأخیر jitterدار دوباره امتحان می‌شوند. پس از پایان retry، پیام با `requeue: false` رد می‌شود. چون در پیکربندی فعلی Dead Letter Exchange/Queue تعریف نشده، پیام ناموفق ممکن است حذف شود. همچنین Publisher فعلی پیام را persistent علامت نمی‌زند و Publisher Confirm یا Outbox پیاده‌سازی نشده است؛ بنابراین Durable بودن صف و Exchange به‌تنهایی تضمین «عدم گم‌شدن پیام» یا سطح اطمینان ۹۹٪ نمی‌دهد. برای تضمین قوی‌تر باید Outbox، پیام persistent، Publisher Confirm و Dead Letter Queue اضافه و آزموده شوند.

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
