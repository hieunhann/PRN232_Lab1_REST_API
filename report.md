# Báo Cáo Phân Tích Yêu Cầu Bài Lab 1: REST API Basics and Deployment (PRN232)

Dựa trên tài liệu đề bài, tôi đã hiểu rõ 100% các yêu cầu kỹ thuật và tiêu chuẩn thiết kế của bài Lab này. Dưới đây là phần tóm tắt và phân tích chi tiết về những việc chúng ta cần làm để hoàn thành bài Lab 1.

## 1. Mục Tiêu Chính
Xây dựng một hệ thống **ASP.NET Core RESTful API** cho hệ thống quản lý học tập (Learning Management System - LMS) áp dụng chặt chẽ kiến trúc **3 lớp (3-layer architecture)**.

## 2. Thiết Kế Cơ Sở Dữ Liệu
Yêu cầu tối thiểu 5 bảng sau:
1. **Semester**: `SemesterId`, `SemesterName`, `StartDate`, `EndDate`
2. **Course**: `CourseId`, `CourseName`, `SemesterId`
3. **Subject**: `SubjectId`, `SubjectCode`, `SubjectName`, `Credit`
4. **Student**: `StudentId`, `FullName`, `Email`, `DateOfBirth`
5. **Enrollment**: `EnrollmentId`, `StudentId`, `CourseId`, `EnrollDate`, `Status`

**Yêu cầu dữ liệu mẫu (Seed Data):** Tối thiểu 5 Semesters, 50 Students, 10 Subjects, 20 Courses, 500 Enrollments.

## 3. Kiến Trúc & Cấu Trúc Project (3-Layer Architecture)
Sử dụng quy tắc đặt tên: `PRN232.[ProjectName].[Layer]`
- **API Layer (Controllers)**: `PRN232.[ProjectName].API` - Chỉ xử lý HTTP Request/Response, KHÔNG chứa logic nghiệp vụ.
- **Service Layer**: `PRN232.[ProjectName].Services` - Xử lý logic nghiệp vụ (Business logic).
- **Repository Layer**: `PRN232.[ProjectName].Repositories` - Tương tác với Database, KHÔNG chứa logic nghiệp vụ.

## 4. Quản Lý Models (4 Loại)
Bắt buộc phân chia rõ ràng 4 loại Model, không được dùng lẫn lộn:
- **Entity Model**: Map trực tiếp với Database (EF Core). Tuyệt đối không trả trực tiếp ra API.
- **Business Model**: Dùng để tính toán/xử lý logic.
- **Request Model**: Nhận input từ Client.
- **Response Model**: Dữ liệu trả ra cho API. Request/Response Model tuyệt đối không được đưa xuống tầng Repository.

## 5. Tiêu Chuẩn REST API
- Sử dụng danh từ số nhiều: `/api/students`, `/api/enrollments/{id}`
- **GET by ID**: Trả về toàn bộ dữ liệu liên quan, chống đệ quy vô hạn (circular references), trả lỗi 404 nếu không tìm thấy.
- **GET Collection (List API)**: Phải hỗ trợ các tính năng nâng cao:
  - **Searching** (`?search=...`): Tìm kiếm theo từ khóa.
  - **Sorting** (`?sort=...`): Sắp xếp tăng/giảm theo nhiều trường.
  - **Paging** (`?page=1&size=10`): Phân trang, có trả về `pagination metadata`.
  - **Selection** (`?fields=id,name`): Chọn các trường cần lấy.
  - **Expansion** (`?expand=student,course`): Load theo dữ liệu của bảng liên quan (Eager Loading).

## 6. Format Response & HTTP Status
Tất cả API phải dùng chung 1 format bọc (wrapper) response:
```json
{
  "success": true/false,
  "message": "...",
  "data": {},
  "errors": null
}
```
Các mã trạng thái HTTP chuẩn: 200, 201, 400, 404, 500.

## 7. Triển Khai (Deployment) & Tài Liệu (Docs)
- **Docker**: Hệ thống bắt buộc chạy với Docker (Database trên Docker Desktop, API trên Docker Container). Cần cấu hình `Dockerfile` và `docker-compose.yml`.
- **Swagger/OpenAPI**: Tích hợp Swagger để mô tả endpoint, test API, và doc các schema request/response/status codes.

## 8. Các Phần KHÔNG Cần Làm (Out of Scope)
- Không yêu cầu làm Đăng nhập/Phân quyền (Authentication/Authorization) và JWT.
- Không cần Validation phức tạp, xử lý Global Exception hay viết Unit/Integration Tests.

---
**Đánh giá mức độ hiểu của tôi:** Tôi hoàn toàn nắm vững yêu cầu kiến trúc (3-tier), cách thiết kế API chuẩn REST (kèm search/sort/pagination/expand), và quy định phân tách Model rất khắt khe của bài Lab này. Cấu hình Docker compose cũng nằm trong khả năng thực hiện của tôi.

**Câu hỏi cho bạn:** 
Bạn dự định đặt tên thay thế cho `[ProjectName]` trong các Project (ví dụ: `PRN232.LMS.API`) là gì? Bạn có muốn tôi tiến hành khởi tạo bộ khung Project (Solution + 3 Class Libraries) và thiết lập file `docker-compose.yml` luôn không?
