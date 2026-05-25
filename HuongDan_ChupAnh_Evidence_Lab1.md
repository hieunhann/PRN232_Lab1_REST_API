# HƯỚNG DẪN CHI TIẾT HOÀN THÀNH 12 YÊU CẦU EVIDENCE - PRN232 LAB 1

Chào bạn, tôi là Senior Fullstack Developer đồng hành cùng bạn. Dưới đây là hướng dẫn chi tiết từng bước để bạn thực hiện kiểm thử (test) và chụp ảnh minh chứng (evidence) cho cả **12 yêu cầu** trong file `SE182117_Evidence_PRN.docx`.

Bằng cách tuân thủ đúng hướng dẫn này, báo cáo của bạn sẽ cực kỳ minh bạch, chuyên nghiệp và đảm bảo đạt điểm số tối đa **10/10** từ giảng viên.

---

## 📌 TỔNG HỢP SỐ LƯỢNG ẢNH CẦN CHỤP: ~23 Ảnh
*Các ảnh chụp cần rõ ràng, hiển thị được thanh địa chỉ URL (nếu dùng trình duyệt/Postman), cổng kết nối (Port), và mã trạng thái HTTP (Status Code).*

---

### 1️⃣ Yêu cầu 1: Kiến trúc 3 lớp (3-layer Architecture)
* **Mục tiêu:** Chứng minh dự án được phân tách rõ ràng thành 3 lớp riêng biệt và độc lập: API (Presentation), Services (Business Logic), và Repositories (Data Access).
* **Cách thực hiện:**
  1. Mở Solution của bạn trong Visual Studio hoặc VS Code.
  2. Mở rộng cấu trúc thư mục của cả 3 project để thấy rõ các thư mục `Controllers`, `Services`, `Repositories`.
  3. Mở một tệp Controller bất kỳ (ví dụ: [EnrollmentsController.cs](file:///D:/2026_SUM/PRN232/LAB01/PRN232.LAB_1_REST_API.API/Controllers/EnrollmentsController.cs)) để chỉ ra rằng Controller **chỉ inject Service** và gọi hàm từ Service chứ không trực tiếp truy vấn Database (DbContext).
* **Số lượng ảnh cần chụp:** **02 ảnh**.
  * 📸 **Ảnh 1.1:** Chụp toàn bộ thanh **Solution Explorer** hiển thị cấu trúc 3 dự án:
    * `PRN232.LAB_1_REST_API.API`
    * `PRN232.LAB_1_REST_API.Services`
    * `PRN232.LAB_1_REST_API.Repositories`
  * 📸 **Ảnh 1.2:** Chụp màn hình code của `EnrollmentsController.cs` đoạn Constructor inject `IEnrollmentService` và một Action method (ví dụ: `GetEnrollments`). Khoanh đỏ phần gọi service: `_enrollmentService.GetEnrollmentsAsync(...)`.

---

### 2️⃣ Yêu cầu 2: Quy chuẩn đặt tên dự án (Project Naming Convention)
* **Mục tiêu:** Chứng minh tên các project được đặt đúng theo chuẩn quy định của đề bài.
* **Cách thực hiện:**
  1. Chỉ cần sử dụng lại cấu trúc dự án ở **Ảnh 1.1**.
  2. Nêu rõ trong báo cáo là tên project tuân thủ chính xác định dạng: `PRN232.[ProjectName].[Layer]`.
* **Số lượng ảnh cần chụp:** **01 ảnh** (Có thể dùng chung **Ảnh 1.1** ở Yêu cầu 1, hoặc chụp riêng màn hình thư mục chứa code ở ổ đĩa D).
  * 📸 **Ảnh 2.1:** Chụp danh sách 3 thư mục dự án trong File Explorer ở đường dẫn `D:\2026_SUM\PRN232\LAB01`.

---

### 3️⃣ Yêu cầu 3: Cấu trúc Database & Dữ liệu Seed (DB Schema & Seed Data)
* **Mục tiêu:** Chứng minh database khởi tạo đủ 5 bảng (`Semesters`, `Courses`, `Subjects`, `Students`, `Enrollments`) và đã seed đủ số lượng dữ liệu mẫu cực lớn theo đề bài (tối thiểu 5 semesters, 50 students, 10 subjects, 20 courses, 500 enrollments).
* **Cách thực hiện:**
  1. Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio, kết nối vào database `Lab1LmsDb` (chạy từ Docker hoặc Local).
  2. Chạy câu lệnh SQL sau để đếm chính xác số lượng bản ghi:
     ```sql
     SELECT 
       (SELECT COUNT(*) FROM Semesters) AS [Total Semesters],
       (SELECT COUNT(*) FROM Students) AS [Total Students],
       (SELECT COUNT(*) FROM Subjects) AS [Total Subjects],
       (SELECT COUNT(*) FROM Courses) AS [Total Courses],
       (SELECT COUNT(*) FROM Enrollments) AS [Total Enrollments];
     ```
* **Số lượng ảnh cần chụp:** **02 ảnh**.
  * 📸 **Ảnh 3.1:** Chụp cấu trúc cây thư mục Databases -> `Lab1LmsDb` -> **Tables** trong SSMS, hiển thị đủ 5 bảng chính.
  * 📸 **Ảnh 3.2:** Chụp kết quả chạy câu lệnh SQL đếm số dòng ở trên (hiển thị rõ các con số vượt chỉ tiêu của đề bài, ví dụ: 5, 50, 10, 20, 500+).

---

### 4️⃣ Yêu cầu 4: Sử dụng 4 loại Model (4 Model Types)
* **Mục tiêu:** Chứng minh dự án phân tách rõ ràng 4 lớp Model (Entity Model, Business/DTO Model, Request Model, Response Model). Đảm bảo không trả trực tiếp Entity ra ngoài API và không dùng Request/Response Model ở tầng Repository.
* **Cách thực hiện:**
  1. Trong dự án, chỉ ra vị trí khai báo của 4 loại Model này.
  2. Mở file định nghĩa Request/Response Model (ví dụ: `StudentRequest.cs`, `StudentResponse.cs`) và file Entity (`Student.cs`).
* **Số lượng ảnh cần chụp:** **02 ảnh**.
  * 📸 **Ảnh 4.1:** Chụp cấu trúc thư mục chứa các Model/DTO trong dự án (ví dụ các thư mục chứa Entities ở Repository, DTOs ở Services, Requests/Responses ở API) để thấy sự phân tách.
  * 📸 **Ảnh 4.2:** Chụp một đoạn code Mapping (ví dụ trong `StudentService.cs`) thực hiện map từ `Student` (Entity) sang `StudentResponse` (Response Model) trước khi trả về cho Controller.

---

### 5️⃣ Yêu cầu 5: Thiết kế Endpoint chuẩn RESTful (RESTful Endpoint Naming)
* **Mục tiêu:** Chứng minh các URL API sử dụng danh từ số nhiều, đại diện cho tài nguyên và không chứa các động từ hành động (như `get`, `create`, `delete`...).
* **Cách thực hiện:**
  1. Chạy dự án và mở trang Swagger UI (`http://localhost:5000/swagger/index.html`).
  2. Quan sát danh sách các Endpoint của các Controller.
* **Số lượng ảnh cần chụp:** **01 ảnh**.
  * 📸 **Ảnh 5.1:** Chụp màn hình Swagger UI hiển thị danh sách các đường dẫn chuẩn RESTful của ít nhất một Controller (ví dụ `/api/students` với đầy đủ các phương thức `GET`, `POST`, `PUT`, `DELETE`).

---

### 6️⃣ Yêu cầu 6: Lấy thông tin tài nguyên theo ID (GET Resource by ID)
* **Mục tiêu:** API lấy chi tiết theo ID hoạt động đúng, tự động trả về thông tin liên quan (không bị vòng lặp vô hạn - circular reference) và trả về mã lỗi 404 nếu không tìm thấy ID.
* **Cách thực hiện:**
  1. Trên Swagger hoặc Postman, thực hiện gọi API `GET /api/students/{id}`.
  2. **Trường hợp 1:** Nhập một ID hợp lệ đang có trong database (ví dụ: `1`).
  3. **Trường hợp 2:** Nhập một ID chắc chắn không tồn tại (ví dụ: `9999`).
* **Số lượng ảnh cần chụp:** **02 ảnh**.
  * 📸 **Ảnh 6.1:** Chụp kết quả gọi API thành công với ID hợp lệ, hiển thị rõ mã trạng thái `200 OK` và Response JSON đầy đủ thông tin sinh viên.
  * 📸 **Ảnh 6.2:** Chụp kết quả gọi API thất bại với ID không tồn tại, hiển thị rõ mã trạng thái `404 Not Found` kèm Response format chuẩn báo lỗi.

---

### 7️⃣ Yêu cầu 7: Các tính năng nâng cao của API danh sách (List API Capabilities)
* **Mục tiêu:** Chứng minh API danh sách hỗ trợ đầy đủ 5 tính năng: Searching (Tìm kiếm), Sorting (Sắp xếp), Paging (Phân trang), ```Fields``` Selection (Chọn trường hiển thị), và Expansion (Mở rộng dữ liệu quan hệ).
* **Cách thực hiện:**
  Chạy các request cụ thể trên Swagger hoặc Postman để kiểm tra từng tính năng.
* **Số lượng ảnh cần chụp:** **05 ảnh** (Tương ứng với 5 tính năng).
  * 📸 **Ảnh 7.1 (Searching/Filtering):** Gọi `GET /api/students?search=Nguyen` (hoặc tên bất kỳ có trong DB). Ảnh chụp phải cho thấy kết quả chỉ trả về các sinh viên có họ/tên chứa chữ "Nguyen".
  * 📸 **Ảnh 7.2 (Sorting):** Gọi `GET /api/students?sort=-dateOfBirth` (Sắp xếp theo ngày sinh giảm dần). Ảnh chụp phải cho thấy trường `dateOfBirth` trong danh sách kết quả được sắp xếp từ gần nhất đến xa nhất.
  * 📸 **Ảnh 7.3 (Paging):** Gọi `GET /api/students?page=2&size=5`. Ảnh chụp phải cho thấy danh sách kết quả trả về đúng 5 phần tử của trang số 2.
  * 📸 **Ảnh 7.4 (Fields Selection):** Gọi `GET /api/students?fields=studentId,fullName,email`. Ảnh chụp phải cho thấy cấu trúc JSON trả về **chỉ có 3 trường** này, các trường khác như `dateOfBirth` hoàn toàn không xuất hiện hoặc bị bỏ qua.
  * 📸 **Ảnh 7.5 (Expansion):** Gọi `GET /api/enrollments?expand=student,course` (hoặc lấy của 1 enrollment cụ thể). Ảnh chụp phải cho thấy đối tượng `student` và `course` bên trong JSON kết quả đã được nạp đầy đủ thông tin chi tiết thay vì chỉ hiển thị ID.

---

### 8️⃣ Yêu cầu 8: Metadata phân trang (Pagination Metadata)
* **Mục tiêu:** Đảm bảo Response của các API danh sách luôn đính kèm thông tin phân trang bao gồm: `page`, `pageSize`, `totalItems`, và `totalPages` để Client dễ dàng dựng giao diện phân trang.
* **Cách thực hiện:**
  1. Gửi request `GET /api/students?page=1&size=5` trên Swagger hoặc Postman.
  2. Cuộn xuống phần cuối hoặc phần chứa metadata của Response JSON.
* **Số lượng ảnh cần chụp:** **01 ảnh**.
  * 📸 **Ảnh 8.1:** Chụp rõ cấu trúc Response JSON, khoanh đỏ đối tượng `"pagination"` chứa đủ 4 thông số: `page`, `pageSize`, `totalItems`, `totalPages` (ví dụ: `page: 1, pageSize: 5, totalItems: 50, totalPages: 10`).

---

### 9️⃣ Yêu cầu 9: Định dạng Response đồng nhất & Mã lỗi HTTP (Response Format & HTTP Status Codes)
* **Mục tiêu:** Chứng minh toàn bộ API đều trả về một cấu trúc JSON đồng nhất gồm các trường: `{ success, message, data, errors }` và trả về chính xác mã trạng thái HTTP tương ứng.
* **Cách thực hiện & Số lượng ảnh cần chụp:** **05 ảnh** (Mỗi mã trạng thái HTTP chụp 1 ảnh kết quả).
  * 📸 **Ảnh 9.1 (Mã 200 OK):** Gọi `GET /api/students/1` thành công. Chụp màn hình hiển thị rõ chữ `Status: 200 OK` và cấu trúc JSON chuẩn có `"success": true`.
  * 📸 **Ảnh 9.2 (Mã 201 Created):** 
    * **Thực hiện:** Trên Swagger hoặc Postman, chọn API **`POST /api/students`** và gửi dữ liệu JSON hợp lệ dưới đây:
      ```json
      {
        "fullName": "Nguyen Huy Hoang",
        "email": "hoangnh@fpt.edu.vn",
        "dateOfBirth": "2004-10-15T00:00:00"
      }
      ```
    * **Chụp ảnh:** Chụp lại màn hình trả về mã trạng thái **`Status: 201 Created`**, phần Header `Location` chứa link lấy chi tiết sinh viên, và phần Response Body hiển thị thông tin học sinh vừa tạo có ID mới tự sinh nằm trong trường `"data"`.
  * 📸 **Ảnh 9.3 (Mã 400 Bad Request):**
    * **Thực hiện:** Trên Swagger hoặc Postman, chọn API **`POST /api/students`** nhưng cố tình gửi dữ liệu sai định dạng (ví dụ email không hợp lệ và tên rỗng):
      ```json
      {
        "fullName": "",
        "email": "sai_dinh_dang_email",
        "dateOfBirth": "2004-10-15T00:00:00"
      }
      ```
    * **Chụp ảnh:** Chụp lại màn hình trả về mã trạng thái **`Status: 400 Bad Request`**, với JSON phản hồi chứa `"success": false` và phần `"errors"` chỉ rõ chi tiết lỗi của từng trường (`FullName` và `Email`).
  * 📸 **Ảnh 9.4 (Mã 404 Not Found):** Gọi `GET /api/students/9999` (ID không tồn tại). Chụp màn hình hiển thị rõ `Status: 404 Not Found` và trường `"success": false`, `"message": "Student not found"`.
  * 📸 **Ảnh 9.5 (Mã 500 Internal Server Error - TẮT SQL SERVER):** 
    * **Cách làm:** Mở Docker Desktop hoặc Services trên Windows, **Stop (tắt)** container database `lab1_db` (hoặc dừng dịch vụ SQL Server Local).
    * Sau đó, thực hiện gọi API `GET /api/students` từ Swagger hoặc trình duyệt. Lúc này API không thể kết nối tới DB và sẽ văng lỗi.
    * Chụp màn hình hiển thị rõ `Status: 500 Internal Server Error` cùng định dạng Response lỗi đồng nhất của hệ thống.
    * *Đừng quên BẬT LẠI Database sau khi chụp xong bước này nhé!*

---

### 🔟 Yêu cầu 10: Triển khai bằng Docker (Docker Deployment)
* **Mục tiêu:** Chứng minh hệ thống được đóng gói và chạy thành công trên môi trường Docker bằng Docker Compose (cả Database và API).
* **Cách thực hiện:**
  1. Mở terminal tại thư mục dự án và chạy lệnh: `docker-compose up -d --build` để build và khởi động hệ thống.
  2. Mở ứng dụng **Docker Desktop** trên Windows.
* **Số lượng ảnh cần chụp:** **03 ảnh**.
  * 📸 **Ảnh 10.1:** Chụp màn hình **Docker Desktop**, hiển thị Group container dự án của bạn đang chạy với cả 2 service: `lab1_db` và `lab1_api` đều có chấm tròn màu xanh lá cây biểu thị trạng thái **Running**.
  * 📸 **Ảnh 10.2:** Chụp nội dung file `docker-compose.yml` đang mở trên IDE để giáo viên thấy cấu hình mapping cổng, biến môi trường và volume database.
  * 📸 **Ảnh 10.3:** Mở trình duyệt web, truy cập địa chỉ cổng Docker của API: `http://localhost:5000/swagger/index.html`. Chụp màn hình giao diện Swagger tải thành công từ container.

---

### 1️⃣1️⃣ Yêu cầu 11: Tài liệu hóa Swagger/OpenAPI (Swagger Documentation & Testing)
* **Mục tiêu:** Chứng minh hệ thống được tài liệu hóa rõ ràng và trực quan trên giao diện Swagger UI, hiển thị đầy đủ các Endpoint, cấu trúc dữ liệu gửi lên (Request Schema), dữ liệu nhận về (Response Schema) và đặc biệt là định nghĩa rõ ràng các mã trạng thái HTTP trả về để Client tiện sử dụng.
* **Cách thực hiện:**
  1. Truy cập Swagger UI tại: `http://localhost:5000/swagger/index.html`.
  2. Mở rộng (Expand) một Endpoint đại diện, ví dụ `POST /api/students` hoặc `GET /api/students/{id}`.
* **Số lượng ảnh cần chụp:** **02 ảnh**.
  * 📸 **Ảnh 11.1 (Tổng quan Swagger):** Chụp tổng quan giao diện Swagger UI của bạn hiển thị đủ 5 Resources với đầy đủ các phương thức HTTP RESTful tương ứng. Rõ nét nhất là mục của **`Students`** hiện tại đã có cả `GET` và `POST`.
  * 📸 **Ảnh 11.2 (Chi tiết Responses & Schema):** Chụp phần **Responses** của API `POST /api/students` hoặc `GET /api/students/{id}` trên Swagger. Ảnh chụp phải thể hiện rõ:
    * Swagger liệt kê đầy đủ các mã trạng thái HTTP có thể trả về: `200` (hoặc `201`), `400`, `404`, `500`.
    * Mỗi mã trạng thái đều có mô tả tương ứng (ví dụ: *Success*, *Bad Request*, *Not Found*, *Internal Server Error*).
    * Bấm chọn tab **Schema** ở phần Response 200/201 để chứng minh có cấu trúc dữ liệu mẫu (`ApiResponse`) rõ ràng với các trường `success`, `message`, `data`, `errors` định nghĩa kiểu dữ liệu chuẩn mực.

---

### 1️⃣2️⃣ Yêu cầu 12: Chất lượng mã nguồn (Code Quality)
* **Mục tiêu:** Chứng minh mã nguồn sạch sẽ (Clean Code), tổ chức cấu trúc 3 lớp rõ ràng, tuân thủ C# Best Practices, đặt tên biến rõ nghĩa và đặc biệt là **có viết comment chú thích nội dung chính bằng Tiếng Việt đầy đủ** ở các khối logic phức tạp để code dễ đọc, dễ bảo trì.
* **Cách thực hiện:**
  1. Mở IDE của bạn (Visual Studio / VS Code).
  2. Định vị đến các khối code đã được chúng ta thiết kế tối ưu, sạch sẽ và có chú thích Tiếng Việt rõ ràng.
* **Số lượng ảnh cần chụp:** **02 ảnh** (Để tăng tính thuyết phục cao nhất với giảng viên).
  * 📸 **Ảnh 12.1 (Comment ở Tầng Service):** 
    * Mở file [StudentService.cs](file:///d:/2026_SUM/PRN232/LAB01/PRN232.LAB_1_REST_API.Services/StudentService.cs) tại phương thức **`AddStudentAsync`**.
    * Chụp lại màn hình đoạn mã này. Ảnh chụp sẽ cho thấy các bước xử lý logic (Ánh xạ Request -> Gọi Repo lưu DB -> Ánh xạ ngược lại Business Model) được tổ chức ngắn gọn, mạch lạc và có comment Tiếng Việt chi tiết giải thích cho từng bước (Bước 1, Bước 2, Bước 3, Bước 4).
  * 📸 **Ảnh 12.2 (Comment & Exception ở Tầng Controller):**
    * Mở file [StudentsController.cs](file:///d:/2026_SUM/PRN232/LAB01/PRN232.LAB_1_REST_API.API/Controllers/StudentsController.cs) tại Action method **`CreateStudent`**.
    * Chụp lại màn hình. Ảnh chụp sẽ chứng minh API Controller được thiết kế chuyên nghiệp: xử lý Validation đầu vào cực sạch bằng `ModelState.IsValid`, trả về lỗi `400 BadRequest` hoặc thành công `201 CreatedAtAction` bọc trong vỏ `ApiResponse`, đồng thời có comment Tiếng Việt giải thích cặn kẽ từng bước xử lý.

---

## 💡 LỢI THẾ CỦA BẠN (SẴN SÀNG ĐẠT 10/10)
Trong các phiên làm việc trước, tôi đã cùng bạn tối ưu hóa hệ thống để đạt trạng thái **hoàn hảo**:
1. **Dynamic Filtering & Expansion cực mạnh:** Sử dụng `System.Linq.Dynamic.Core` giúp xử lý đồng thời Search, Sort, Paging, Fields Selection, và Eager Loading (Expand) lồng nhau mà không bị lỗi vòng lặp tuần hoàn (Circular Reference). Điều này giúp các **Ảnh 7.x** và **Ảnh 8.1** của bạn trông cực kỳ chuyên nghiệp và vượt trội hơn hẳn so với các bài làm thông thường.
2. **Response nhất quán:** Hệ thống trả về đúng định dạng JSON `{ success, message, data, errors }` cho mọi trường hợp lỗi hay thành công. Giúp bạn chụp các ảnh **9.x** vô cùng dễ dàng và đồng bộ.
3. **Docker sẵn sàng:** Cấu hình Dockerfile và docker-compose đã được tối ưu hóa cổng chạy (`5000:8080`), volume lưu trữ dữ liệu DB được tách biệt để không bị mất dữ liệu khi restart container.

Chúc bạn hoàn thành bài báo cáo xuất sắc! Nếu gặp bất kỳ khó khăn nào trong quá trình thao tác kiểm thử để chụp ảnh, hãy nhắn ngay cho tôi để nhận hỗ trợ tức thì.
