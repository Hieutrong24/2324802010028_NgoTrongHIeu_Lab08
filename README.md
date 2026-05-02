Dự án ASC Web - 2324802010028 - Ngô Trọng Hiếu

Em xin chào thầy, dưới đây là phần mô tả tổng quan dự án ASC Web mà em đã thực hiện thông qua 7 bài lab.

# 🚗 ASC Web - Automobile Service Center

## 1. Giới thiệu dự án ASC Web

**ASC Web** là một ứng dụng web quản lý trung tâm dịch vụ ô tô, được xây dựng bằng **ASP.NET Core MVC .NET 8**. Dự án mô phỏng quy trình vận hành của một hệ thống Automobile Service Center, bao gồm quản lý người dùng, khách hàng, kỹ sư dịch vụ, dữ liệu cấu hình, yêu cầu dịch vụ, thông báo và khuyến mãi.

Dự án được thực hiện theo từng giai đoạn thông qua 7 bài lab. Mỗi lab bổ sung thêm một phần kiến thức quan trọng trong quá trình xây dựng ứng dụng web thực tế, từ thiết kế giao diện ban đầu, tổ chức kiến trúc nhiều tầng, kiểm thử, bảo mật, quản lý tài khoản, quản lý Master Data cho đến xử lý yêu cầu dịch vụ và cache dữ liệu bằng Redis.

---

## 2. Mục tiêu của dự án

Mục tiêu của dự án là xây dựng một hệ thống web có cấu trúc rõ ràng, có khả năng mở rộng và phù hợp với mô hình ứng dụng thực tế. Thông qua dự án này, em đã thực hiện các mục tiêu chính sau:

- Xây dựng ứng dụng web theo mô hình **ASP.NET Core MVC**.
- Áp dụng kiến trúc nhiều tầng để tách biệt giao diện, dữ liệu và nghiệp vụ.
- Sử dụng **Entity Framework Core** để thao tác với cơ sở dữ liệu.
- Áp dụng **Repository Pattern** và **UnitOfWork Pattern**.
- Sử dụng **ASP.NET Core Identity** để xác thực và phân quyền.
- Tích hợp đăng nhập bằng **Google OAuth 2.0**.
- Xây dựng chức năng quản lý tài khoản, khách hàng và kỹ sư dịch vụ.
- Xây dựng chức năng quản lý Master Data và import dữ liệu từ Excel.
- Xây dựng quy trình tạo và theo dõi yêu cầu dịch vụ.
- Sử dụng **Redis Cache** để lưu trữ dữ liệu cấu hình dùng chung.
- Thực hiện kiểm thử đơn vị bằng **xUnit** và **Moq**.
- Thiết kế giao diện bằng **Materialize CSS**.

---

## 3. Công nghệ sử dụng

| Công nghệ | Mục đích sử dụng |
|---|---|
| **ASP.NET Core MVC .NET 8** | Xây dựng ứng dụng web theo mô hình MVC |
| **Entity Framework Core** | Truy vấn và thao tác với cơ sở dữ liệu |
| **SQL Server LocalDB** | Lưu trữ dữ liệu hệ thống |
| **ASP.NET Core Identity** | Đăng nhập, đăng xuất, phân quyền người dùng |
| **Google OAuth 2.0** | Đăng nhập bằng tài khoản Google |
| **xUnit** | Viết unit test |
| **Moq** | Mock dependency trong kiểm thử |
| **Materialize CSS** | Xây dựng giao diện người dùng |
| **MailKit** | Gửi email reset password và thông báo |
| **Redis / StackExchangeRedisCache** | Cache dữ liệu Master Data |
| **AutoMapper** | Ánh xạ dữ liệu giữa Entity và ViewModel |
| **EPPlus** | Import dữ liệu từ file Excel |
| **DataTables** | Tìm kiếm, phân trang và hiển thị bảng dữ liệu |

---

## 4. Cấu trúc solution

Dự án được tổ chức theo kiến trúc nhiều tầng như sau:

```text
ASC.Web
ASC.Model
ASC.DataAccess
ASC.Business
ASC.Utilities
ASC.Tests
```

### Vai trò từng project

| Project | Vai trò |
|---|---|
| `ASC.Web` | Tầng giao diện, Controller, View, cấu hình Identity, routing và các Area |
| `ASC.Model` | Chứa các Entity/Model ánh xạ với bảng trong database |
| `ASC.DataAccess` | Chứa Repository Pattern, UnitOfWork Pattern và xử lý truy cập dữ liệu |
| `ASC.Business` | Chứa các nghiệp vụ chính của hệ thống |
| `ASC.Utilities` | Chứa các tiện ích dùng chung như Session Extension |
| `ASC.Tests` | Chứa các unit test sử dụng xUnit và Moq |

---

## 5. Nội dung đã thực hiện theo từng Lab

## Lab 1 - Designing a Modern Real-World Web Application

Trong Lab 1, em đã xây dựng nền tảng ban đầu cho dự án:

- Tạo project `ASC.Web` bằng **ASP.NET Core MVC .NET 8**.
- Cấu hình `appsettings.json` và `ApplicationSettings`.
- Sử dụng `IOptions<ApplicationSettings>` để đọc cấu hình.
- Tạo các service mẫu:
  - `IEmailSender`
  - `ISmsSender`
  - `AuthMessageSender`
- Thiết kế layout bằng **Materialize CSS**.
- Tạo các layout:
  - `_MasterLayout.cshtml`
  - `_Layout.cshtml`
  - `_SecureLayout.cshtml`
- Xây dựng trang chủ `Home/Index`.
- Xây dựng Dashboard ban đầu cho khu vực bảo mật.

---

## Lab 2 - Domain-Driven Architecture, Repository Pattern và UnitOfWork Pattern

Trong Lab 2, em đã tổ chức lại dự án theo kiến trúc nhiều tầng:

- Tạo các project:
  - `ASC.Model`
  - `ASC.DataAccess`
  - `ASC.Business`
  - `ASC.Utilities`
- Tạo các Entity:
  - `BaseEntity`
  - `ServiceRequest`
  - `MasterDataKey`
  - `MasterDataValue`
  - `Product`
- Xây dựng **Repository Pattern**:
  - `IRepository`
  - `Repository`
- Xây dựng **UnitOfWork Pattern**:
  - `IUnitOfWork`
  - `UnitOfWork`
- Tạo `ApplicationDbContext`.
- Cấu hình kết nối **SQL Server LocalDB**.
- Tạo migration và cập nhật database.
- Seed dữ liệu cấu hình ban đầu cho hệ thống.

---

## Lab 3 - Test-Driven Architecture với xUnit và Moq

Trong Lab 3, em đã thực hiện kiểm thử đơn vị:

- Tạo project `ASC.Tests`.
- Cài đặt các package:
  - `xUnit`
  - `xunit.runner.visualstudio`
  - `Moq`
- Viết test cho `HomeController`.
- Mock `IOptions<ApplicationSettings>`.
- Cấu hình Session trong ASP.NET Core.
- Tạo `SessionExtensions`.
- Tạo `FakeSession` để kiểm thử Session.
- Kiểm tra các trường hợp:
  - Controller trả về đúng `ViewResult`.
  - Model không bị null.
  - Không có lỗi validation.
  - Session lưu được dữ liệu cần thiết.

---

## Lab 4 - Securing the Application with ASP.NET Core Identity and OAuth 2.0

Trong Lab 4, em đã bổ sung bảo mật và phân quyền:

- Cài đặt và cấu hình **ASP.NET Core Identity**.
- Sử dụng `IdentityUser` và `IdentityRole`.
- Tạo tài khoản Admin mặc định khi chạy ứng dụng.
- Tạo các controller nền:
  - `BaseController`
  - `AnonymousController`
- Tạo Area `ServiceRequests`.
- Tạo Dashboard yêu cầu đăng nhập.
- Cấu hình Login, Logout, Forgot Password và Reset Password.
- Gửi email reset password bằng **MailKit**.
- Tạo hệ thống menu động theo role bằng `Navigation.json`.
- Xây dựng `LeftNavigationViewComponent`.
- Phân quyền menu theo các role:
  - Admin
  - Engineer
  - User

---

## Lab 5 - Account Management

Trong Lab 5, em đã xây dựng chức năng quản lý tài khoản:

- Tích hợp đăng nhập bằng **Google OAuth 2.0**.
- Cấu hình `ClientId` và `ClientSecret` trong `appsettings.json`.
- Cài đặt `Microsoft.AspNetCore.Authentication.Google`.
- Cập nhật trang Login để hỗ trợ đăng nhập Google.
- Xử lý `ExternalLogin` để tạo tài khoản User khi đăng nhập bằng Gmail.
- Tạo Area `Accounts`.
- Quản lý Service Engineers:
  - Thêm mới kỹ sư dịch vụ.
  - Cập nhật thông tin kỹ sư.
  - Active/Deactive tài khoản.
  - Gửi email thông báo khi cập nhật.
- Quản lý Customers:
  - Hiển thị danh sách khách hàng.
  - Cập nhật trạng thái Active/Deactive.
- Xây dựng chức năng Profile:
  - Xem thông tin tài khoản.
  - Cập nhật UserName.
- Tích hợp DataTables cho các bảng danh sách.

---

## Lab 6 - Master Data Management

Trong Lab 6, em đã xây dựng chức năng quản lý dữ liệu cấu hình dùng chung:

- Tạo `IMasterDataOperations`.
- Tạo `MasterDataOperations`.
- Đăng ký service trong `Program.cs`.
- Cài đặt **AutoMapper**.
- Tạo Area `Configuration`.
- Xây dựng chức năng Master Keys:
  - Thêm mới Master Key.
  - Cập nhật Master Key.
  - Active/Inactive Master Key.
- Xây dựng chức năng Master Values:
  - Thêm mới Master Value.
  - Cập nhật Master Value.
  - Gán Master Value theo Master Key.
- Cài đặt **EPPlus**.
- Import Master Data từ file Excel.
- Cập nhật menu Master Data trong sidebar.

---

## Lab 7 - Service Request Management

Trong Lab 7, em đã xây dựng chức năng quản lý yêu cầu dịch vụ:

- Cài đặt và chạy **Redis**.
- Cấu hình Redis cache trong ASP.NET Core.
- Tạo `MasterDataCache`.
- Tạo `IMasterDataCacheOperations`.
- Tạo `MasterDataCacheOperations`.
- Cache dữ liệu Master Data vào Redis.
- Cập nhật model `ServiceRequest`.
- Tạo migration cập nhật bảng `ServiceRequests`.
- Tạo `IServiceRequestOperations`.
- Tạo `ServiceRequestOperations`.
- Tạo màn hình New Service Request.
- Lấy dữ liệu dropdown từ Redis cache:
  - Vehicle Type
  - Requested Service
  - Service Engineer
- Tạo Dashboard Service Request theo role:
  - Admin xem tất cả yêu cầu.
  - User chỉ xem yêu cầu của chính mình.
  - Engineer chỉ xem yêu cầu được gán cho mình.
- Tạo thống kê:
  - Total Requests
  - New Requests
  - In Progress Requests
  - Completed Requests

---

## 6. Các chức năng chính đã hoàn thành

Dự án hiện đã hoàn thành các chức năng chính sau:

- Trang chủ giới thiệu hệ thống ASC Web.
- Đăng nhập bằng tài khoản Identity.
- Đăng nhập bằng Google OAuth.
- Đăng xuất.
- Quên mật khẩu và đặt lại mật khẩu.
- Gửi email bằng MailKit.
- Phân quyền theo role Admin, Engineer, User.
- Menu động theo quyền người dùng.
- Quản lý khách hàng.
- Quản lý kỹ sư dịch vụ.
- Cập nhật profile người dùng.
- Quản lý Master Keys.
- Quản lý Master Values.
- Import Master Data từ Excel.
- Cache Master Data bằng Redis.
- Tạo yêu cầu dịch vụ mới.
- Dashboard quản lý Service Request theo từng role.
- Quản lý Service Notifications.
- Quản lý Promotions.
- Unit test bằng xUnit và Moq.

---

## 7. Tài khoản đăng nhập mẫu

Tài khoản Admin mặc định được tạo khi chạy ứng dụng:

```text
Email: admin@asc.com
Password: Admin@123
Role: Admin
```

Tài khoản Engineer có thể được tạo trong menu:

```text
User Administration → Service Engineers
```

Tài khoản User/Customer có thể được tạo thông qua đăng nhập Google hoặc đăng ký bằng tài khoản người dùng.

---

## 8. Kết luận

Thông qua 7 bài lab, em đã xây dựng được một hệ thống ASP.NET Core MVC hoàn chỉnh cho bài toán quản lý trung tâm dịch vụ ô tô. Dự án đã áp dụng nhiều kiến thức quan trọng như kiến trúc nhiều tầng, Repository Pattern, UnitOfWork Pattern, Entity Framework Core, ASP.NET Core Identity, Google OAuth, kiểm thử đơn vị, quản lý dữ liệu Master Data, Redis cache và quản lý yêu cầu dịch vụ theo phân quyền.

Dự án ASC Web giúp em hiểu rõ hơn về cách xây dựng một ứng dụng web thực tế, có cấu trúc rõ ràng, dễ mở rộng và có thể phát triển thêm nhiều chức năng trong tương lai.


##Link video demo:
- https://youtu.be/2vAXoTrI1po?feature=shared

