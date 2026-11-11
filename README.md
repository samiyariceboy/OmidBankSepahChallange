# OmidBankSepah

## پیش‌نیازها

- نصب .NET 9 SDK
- دسترسی به SQL Server
- در دسترس بودن Redis در `localhost:6379` یا تنظیم مقدار `Redis:ConnectionString`
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

در این مرحله پیام‌رسان یا Event Bus وجود ندارد و فرمان رزرو به‌صورت هم‌زمان با gRPC اجرا می‌شود. در صورت قطع ارتباط یا در دسترس نبودن پایگاه‌داده‌ی Order، سفارش می‌تواند `Pending` بماند؛ همان درخواست را با همان کلید idempotency دوباره ارسال کنید. اگر موجودی کم شده باشد ولی وضعیت نهایی سفارش ذخیره نشده باشد، تلاش مجدد نتیجه‌ی ذخیره‌شده‌ی MasterData را بازیابی می‌کند. هنوز فرایند آزادسازی جبرانی برای سفارش‌هایی که هیچ‌وقت دوباره ارسال نمی‌شوند وجود ندارد.

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

Redis را اجرا کنید و هر سرویس را در یک ترمینال جداگانه بالا بیاورید:

```powershell
dotnet run --project src\MasterData\OmidBankSepah.MasterData.Grpc\OmidBankSepah.MasterData.Grpc.csproj
dotnet run --project src\MasterData\OmidBankSepah.MasterData.Presentation\OmidBankSepah.MasterData.Presentation.csproj
dotnet run --project src\Order\OmidBankSepah.Order.Presentation\OmidBankSepah.Order.Presentation.csproj
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
