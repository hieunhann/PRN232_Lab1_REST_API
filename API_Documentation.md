# LMS System RESTful API Documentation

Welcome to the **Learning Management System (LMS) API** documentation. This document provides a comprehensive reference for interacting with all resources in the system. The API has been designed following strict **RESTful architecture design principles** and features uniform responses, standardized error handling, and robust query capabilities.

---

## 📌 Architectural Standards & Features

### 1. Base URL
All API requests in local development or Docker containerized environments should be sent to:
* **Local HTTP:** `http://localhost:5000/api`
* **Docker HTTP:** `http://localhost:5000/api`

### 2. HTTP Methods & REST Semantics
The API relies on standard HTTP methods to represent actions on resources:
* **`GET`**: Retrieve a resource or a collection of resources. (Safe & Idempotent)
* **`POST`**: Create a new resource. (Non-idempotent)
* **`PUT`**: Fully update an existing resource or replace it. (Idempotent)
* **`DELETE`**: Remove a resource. (Idempotent)

### 3. Uniform Response Envelope
To ensure client applications can parse responses easily and consistently, **all** API endpoints wrap their payload inside a standard response envelope:

#### Successful Response (`200 OK` or `201 Created`)
```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": { ... } // Single object, list of objects, or customized payload
}
```

#### Paginated Successful Response (`200 OK`)
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 5,
    "totalItems": 50,
    "totalPages": 10
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [ ... ] // Array of shaped items
}
```

#### Error Response (`400 Bad Request`, `404 Not Found`, or `500 Internal Server Error`)
```json
{
  "success": false,
  "message": "Detailed error message or resource-specific failure description",
  "errors": { ... } // Detailed validation errors or HTTP Status details
}
```

### 4. Advanced Query Parameters (List Capabilities)
All collection-based `GET` endpoints (e.g., `/api/students`, `/api/courses`) support the following query string parameters to enable high performance and flexibility:

| Parameter | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| **`search`** | `string` | Perform global text searches across primary text fields of the resource. | `?search=Nguyen` |
| **`sort`** | `string` | Sort results. Use a prefix `-` for descending order. Separated by comma for multi-sorting. | `?sort=-dateOfBirth` |
| **`page`** | `int` | The page number to fetch (1-indexed). Defaults to `1`. | `?page=2` |
| **`size`** | `int` | The number of records per page. Defaults to `10`. | `?size=5` |
| **`fields`** | `string` | Data Shaping / Field selection. Returns only specified fields to minimize network payload. | `?fields=studentId,fullName,email` |
| **`expand`** | `string` | Expand / Eagerly load related navigation properties. | `?expand=Enrollments.Course` |
| **`filter`** | `string` | Apply specific criteria expressions (e.g., field-level operations). | `?filter=dateOfBirth>2000-01-01` |

---

## 👥 Resource: Students (`/api/students`)

Manages student records, their demographic details, and registrations.

### 1. Get List of Students
* **Method:** `GET`
* **Path:** `/api/students`
* **Description:** Retrieve a paginated list of students. Supports search, sort, pagination, dynamic field shaping, relation expansion, and filtering.
* **Query Examples:**
  * Search by name: `/api/students?search=Hoang`
  * Sort by date of birth descending, page size 5: `/api/students?sort=-dateOfBirth&page=1&size=5`
  * Select specific fields: `/api/students?fields=studentId,fullName,email`
  * Expand Enrollments and Courses: `/api/students?expand=Enrollments.Course`

#### Example Response (`200 OK` with pagination & field shaping `fields=studentId,fullName,email`):
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 2,
    "totalItems": 52,
    "totalPages": 26
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [
    {
      "studentId": 1,
      "fullName": "Nguyen Van A",
      "email": "anv@fpt.edu.vn"
    },
    {
      "studentId": 2,
      "fullName": "Tran Thi B",
      "email": "btt@fpt.edu.vn"
    }
  ]
}
```

---

### 2. Get Student by ID
* **Method:** `GET`
* **Path:** `/api/students/{id}`
* **Description:** Get detailed information of a specific student by their primary key.
* **Path Parameters:**
  * `id` (integer, required) - The unique ID of the student.
* **Query Parameters:**
  * `expand` (string, optional) - Navigation properties to include. Defaults to `"Enrollments.Course"`.

#### Example Request:
`GET http://localhost:5000/api/students/1?expand=Enrollments.Course`

#### Example Response (`200 OK` - Success):
```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {
    "studentId": 1,
    "fullName": "Nguyen Van A",
    "email": "anv@fpt.edu.vn",
    "dateOfBirth": "2003-05-15T00:00:00",
    "enrollments": [
      {
        "enrollmentId": 12,
        "studentId": 1,
        "courseId": 3,
        "enrollmentDate": "2026-01-10T08:30:00",
        "course": {
          "courseId": 3,
          "courseName": "Advanced Web Programming",
          "semesterId": 2
        }
      }
    ]
  }
}
```

#### Example Response (`404 Not Found` - Non-existent ID):
```json
{
  "success": false,
  "message": "Student not found",
  "errors": "404 Not Found"
}
```

---

### 3. Create Student
* **Method:** `POST`
* **Path:** `/api/students`
* **Description:** Create a new student. Automatic validation ensures all constraints are met.
* **Request Headers:**
  * `Content-Type: application/json`
* **Request Body:**
  * `fullName` (string, required, min length 2): Full name of the student.
  * `email` (string, required, valid email format): Contact email address.
  * `dateOfBirth` (string, required, ISO-8601 format): Student's birthdate.

#### Example Request Body:
```json
{
  "fullName": "Nguyen Huy Hoang",
  "email": "hoangnh@fpt.edu.vn",
  "dateOfBirth": "2004-10-15T00:00:00"
}
```

#### Example Response (`201 Created` - Success):
* **Headers:** `Location: http://localhost:5000/api/students/53`
```json
{
  "success": true,
  "message": "Tạo mới học sinh thành công!",
  "data": {
    "studentId": 53,
    "fullName": "Nguyen Huy Hoang",
    "email": "hoangnh@fpt.edu.vn",
    "dateOfBirth": "2004-10-15T00:00:00",
    "enrollments": []
  }
}
```

#### Example Response (`400 Bad Request` - Data Validation Failed):
```json
{
  "success": false,
  "message": "Dữ liệu gửi lên không hợp lệ. Vui lòng kiểm tra lại!",
  "errors": {
    "Email": [
      "The Email field is not a valid e-mail address."
    ],
    "FullName": [
      "The FullName field is required."
    ]
  }
}
```

---

### 4. Update Student
* **Method:** `PUT`
* **Path:** `/api/students/{id}`
* **Description:** Fully update an existing student by ID.
* **Path Parameters:**
  * `id` (integer, required) - The unique ID of the student to modify.
* **Request Body:** Same schema as the Create Student payload.

#### Example Request:
`PUT http://localhost:5000/api/students/53`
```json
{
  "fullName": "Nguyen Huy Hoang Updated",
  "email": "hoangnh.updated@fpt.edu.vn",
  "dateOfBirth": "2004-10-15T00:00:00"
}
```

#### Example Response (`200 OK` - Success):
```json
{
  "success": true,
  "message": "Cập nhật thông tin học sinh thành công!",
  "data": {
    "studentId": 53,
    "fullName": "Nguyen Huy Hoang Updated",
    "email": "hoangnh.updated@fpt.edu.vn",
    "dateOfBirth": "2004-10-15T00:00:00"
  }
}
```

---

### 5. Delete Student
* **Method:** `DELETE`
* **Path:** `/api/students/{id}`
* **Description:** Permanently remove a student record by ID.
* **Path Parameters:**
  * `id` (integer, required) - Student ID to delete.

#### Example Request:
`DELETE http://localhost:5000/api/students/53`

#### Example Response (`200 OK` - Success):
```json
{
  "success": true,
  "message": "Xóa học sinh thành công!"
}
```

#### Example Response (`404 Not Found`):
```json
{
  "success": false,
  "message": "Học sinh không tồn tại hoặc không thể xóa!",
  "errors": "404 Not Found"
}
```

---

### 6. Get Courses of a Student
* **Method:** `GET`
* **Path:** `/api/students/{studentId}/courses`
* **Description:** Nested resource endpoint to retrieve all courses a specific student has registered/enrolled in.
* **Path Parameters:**
  * `studentId` (integer, required) - The student's ID.

#### Example Request:
`GET http://localhost:5000/api/students/1/courses`

#### Example Response (`200 OK` - Success):
```json
{
  "success": true,
  "message": "Lấy danh sách khóa học của học sinh thành công!",
  "data": [
    {
      "courseId": 3,
      "courseName": "Advanced Web Programming",
      "semesterId": 2
    },
    {
      "courseId": 5,
      "courseName": "Introduction to Databases",
      "semesterId": 2
    }
  ]
}
```

---

## 📚 Resource: Courses (`/api/courses`)

Represents educational courses offered in specific semesters.

### 1. Get List of Courses
* **Method:** `GET`
* **Path:** `/api/courses`
* **Description:** Retrieve a paginated list of courses with filtering, sorting, and dynamic fields selection.
* **Query Examples:**
  * Filter by semester: `/api/courses?filter=semesterId==2`
  * Eager load details: `/api/courses?expand=Semester`

#### Example Response (`200 OK`):
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 20,
    "totalPages": 2
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [
    {
      "courseId": 1,
      "courseName": "Introduction to Cloud Computing",
      "semesterId": 1
    }
  ]
}
```

---

### 2. Get Course by ID
* **Method:** `GET`
* **Path:** `/api/courses/{id}`
* **Description:** Get specific course details.
* **Query Parameters:**
  * `expand` (string, optional) - Navigation properties. Defaults to `"Enrollments.Student"`.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {
    "courseId": 1,
    "courseName": "Introduction to Cloud Computing",
    "semesterId": 1,
    "enrollments": [
      {
        "enrollmentId": 101,
        "studentId": 2,
        "courseId": 1,
        "enrollmentDate": "2026-01-15T09:00:00",
        "student": {
          "studentId": 2,
          "fullName": "Tran Thi B",
          "email": "btt@fpt.edu.vn"
        }
      }
    ]
  }
}
```

---

### 3. Create Course
* **Method:** `POST`
* **Path:** `/api/courses`
* **Request Body:**
  * `courseName` (string, required): Name of the course.
  * `semesterId` (integer, required): Target Semester ID.

#### Example Request Body:
```json
{
  "courseName": "Mobile Application Development",
  "semesterId": 2
}
```

#### Example Response (`201 Created`):
```json
{
  "success": true,
  "message": "Tạo khóa học thành công!",
  "data": {
    "courseId": 21,
    "courseName": "Mobile Application Development",
    "semesterId": 2
  }
}
```

---

### 4. Update Course
* **Method:** `PUT`
* **Path:** `/api/courses/{id}`
* **Request Body:** Same as Create Course.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Cập nhật khóa học thành công!",
  "data": {
    "courseId": 21,
    "courseName": "Mobile Application Development (iOS & Android)",
    "semesterId": 2
  }
}
```

---

### 5. Delete Course
* **Method:** `DELETE`
* **Path:** `/api/courses/{id}`

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Xóa khóa học thành công!"
}
```

---

### 6. Get Enrolled Students in a Course
* **Method:** `GET`
* **Path:** `/api/courses/{courseId}/students`
* **Description:** Retrieve all students registered for this specific course. Returns a clean, flat JSON collection including enrollment details.
* **Path Parameters:**
  * `courseId` (integer, required) - The unique course identifier.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Lấy danh sách sinh viên của khóa học thành công!",
  "data": [
    {
      "studentId": 2,
      "fullName": "Tran Thi B",
      "email": "btt@fpt.edu.vn",
      "dateOfBirth": "2003-08-20T00:00:00",
      "enrollmentStatus": "Active",
      "enrollDate": "2026-01-15T09:00:00"
    }
  ]
}
```

---

### 7. Get Enrollments of a Course
* **Method:** `GET`
* **Path:** `/api/courses/{id}/enrollments`
* **Description:** Lấy danh sách toàn bộ các lượt đăng ký (Enrollments) của một khóa học, hỗ trợ mở rộng thông tin chi tiết của sinh viên qua tham số `expand=student`.
* **Path Parameters:**
  * `id` (integer, required) - ID của khóa học.
* **Query Parameters:**
  * `expand` (string, optional) - Truyền vào `student` để tải kèm thông tin chi tiết của sinh viên.

#### Example Response (`200 OK` with `expand=student`):
```json
{
  "success": true,
  "message": "Lấy danh sách lượt đăng ký của khóa học thành công!",
  "data": [
    {
      "enrollmentId": 1,
      "studentId": 1,
      "courseId": 20,
      "enrollDate": "2026-01-11T08:30:00",
      "status": "Active",
      "student": {
        "studentId": 1,
        "fullName": "Student Name 1",
        "email": "student1@fpt.edu.vn",
        "dateOfBirth": "2004-01-11T00:00:00"
      }
    },
    {
      "enrollmentId": 11,
      "studentId": 5,
      "courseId": 20,
      "enrollDate": "2026-01-15T09:00:00",
      "status": "Completed",
      "student": {
        "studentId": 5,
        "fullName": "Student Name 5",
        "email": "student5@fpt.edu.vn",
        "dateOfBirth": "2004-02-20T00:00:00"
      }
    }
  ]
}
```

---

## 📝 Resource: Enrollments (`/api/enrollments`)

Acts as the associative entity that records which students are registered in which courses, including their registration date.

### 1. Get List of Enrollments
* **Method:** `GET`
* **Path:** `/api/enrollments`
* **Query Examples:**
  * Eager loading: `/api/enrollments?expand=Student,Course`
  * Pagination: `/api/enrollments?page=1&size=10`

#### Example Response (`200 OK` with nested expansion):
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 1,
    "totalItems": 512,
    "totalPages": 512
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [
    {
      "enrollmentId": 1,
      "studentId": 1,
      "courseId": 3,
      "enrollmentDate": "2026-01-10T08:30:00",
      "student": {
        "studentId": 1,
        "fullName": "Nguyen Van A",
        "email": "anv@fpt.edu.vn"
      },
      "course": {
        "courseId": 3,
        "courseName": "Advanced Web Programming"
      }
    }
  ]
}
```

---

### 2. Get Enrollment by ID
* **Method:** `GET`
* **Path:** `/api/enrollments/{id}`
* **Path Parameters:**
  * `id` (integer, required) - Enrollment primary key.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {
    "enrollmentId": 1,
    "studentId": 1,
    "courseId": 3,
    "enrollmentDate": "2026-01-10T08:30:00"
  }
}
```

---

### 3. Create Enrollment
* **Method:** `POST`
* **Path:** `/api/enrollments`
* **Request Body:**
  * `studentId` (integer, required): Valid student identifier.
  * `courseId` (integer, required): Valid course identifier.
  * `enrollmentDate` (string, required): ISO-8601 date.

#### Example Request Body:
```json
{
  "studentId": 1,
  "courseId": 5,
  "enrollmentDate": "2026-05-25T08:00:00"
}
```

#### Example Response (`201 Created`):
```json
{
  "success": true,
  "message": "Đăng ký môn học thành công!",
  "data": {
    "enrollmentId": 513,
    "studentId": 1,
    "courseId": 5,
    "enrollmentDate": "2026-05-25T08:00:00"
  }
}
```

---

### 4. Update Enrollment
* **Method:** `PUT`
* **Path:** `/api/enrollments/{id}`

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Cập nhật đăng ký học thành công!",
  "data": {
    "enrollmentId": 513,
    "studentId": 1,
    "courseId": 6,
    "enrollmentDate": "2026-05-25T08:00:00"
  }
}
```

---

### 5. Delete Enrollment
* **Method:** `DELETE`
* **Path:** `/api/enrollments/{id}`

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Xóa đăng ký môn học thành công!"
}
```

---

### 6. Get Classmate Students (by Enrollment)
* **Method:** `GET`
* **Path:** `/api/enrollments/{enrollmentId}/students`
* **Description:** Retrieve all students who share the same class/course as the specified enrollment record.
* **Path Parameters:**
  * `enrollmentId` (integer, required) - Enrollment ID.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Lấy danh sách sinh viên cùng lớp thành công!",
  "data": [
    {
      "studentId": 1,
      "fullName": "Nguyen Van A",
      "email": "anv@fpt.edu.vn"
    },
    {
      "studentId": 5,
      "fullName": "Le Hoang C",
      "email": "clh@fpt.edu.vn"
    }
  ]
}
```

---

### 7. Get Course with all Enrolled Students
* **Method:** `GET`
* **Path:** `/api/enrollments/{enrollmentId}/course`
* **Description:** Fetch course details, including its target semester, and the distinct list of all students participating in this course.
* **Path Parameters:**
  * `enrollmentId` (integer, required) - Enrollment ID.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Lấy khóa học theo enrollment thành công!",
  "data": {
    "courseId": 3,
    "courseName": "Advanced Web Programming",
    "semesterId": 2,
    "semester": {
      "semesterId": 2,
      "semesterName": "Spring 2026",
      "startDate": "2026-01-05T00:00:00",
      "endDate": "2026-04-30T00:00:00"
    },
    "students": [
      {
        "studentId": 1,
        "fullName": "Nguyen Van A",
        "email": "anv@fpt.edu.vn",
        "dateOfBirth": "2003-05-15T00:00:00"
      },
      {
        "studentId": 10,
        "fullName": "Vu Minh D",
        "email": "dvm@fpt.edu.vn",
        "dateOfBirth": "2003-12-01T00:00:00"
      }
    ]
  }
}
```

---

## 📅 Resource: Semesters (`/api/semesters`)

Manages the academic terms/semesters.

### 1. Get List of Semesters
* **Method:** `GET`
* **Path:** `/api/semesters`

#### Example Response (`200 OK`):
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 5,
    "totalPages": 1
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [
    {
      "semesterId": 1,
      "semesterName": "Fall 2025",
      "startDate": "2025-09-01T00:00:00",
      "endDate": "2025-12-25T00:00:00"
    }
  ]
}
```

---

### 2. Get Semester by ID
* **Method:** `GET`
* **Path:** `/api/semesters/{id}`

---

### 3. Create Semester
* **Method:** `POST`
* **Path:** `/api/semesters`
* **Request Body:**
  * `semesterName` (string, required): Term name (e.g. "Summer 2026").
  * `startDate` (string, required): Term start date.
  * `endDate` (string, required): Term end date.

#### Example Request Body:
```json
{
  "semesterName": "Summer 2026",
  "startDate": "2026-05-05T00:00:00",
  "endDate": "2026-08-30T00:00:00"
}
```

#### Example Response (`201 Created`):
```json
{
  "success": true,
  "message": "Tạo học kỳ thành công!",
  "data": {
    "semesterId": 6,
    "semesterName": "Summer 2026",
    "startDate": "2026-05-05T00:00:00",
    "endDate": "2026-08-30T00:00:00"
  }
}
```

---

### 4. Update Semester
* **Method:** `PUT`
* **Path:** `/api/semesters/{id}`

---

### 5. Delete Semester
* **Method:** `DELETE`
* **Path:** `/api/semesters/{id}`

---

### 6. Get Courses by Semester
* **Method:** `GET`
* **Path:** `/api/semesters/{semesterId}/courses`
* **Description:** Retrieve all active courses scheduled for this specific academic term.
* **Path Parameters:**
  * `semesterId` (integer, required) - Semester ID.

#### Example Response (`200 OK`):
```json
{
  "success": true,
  "message": "Lấy danh sách khóa học của học kỳ thành công!",
  "data": [
    {
      "courseId": 3,
      "courseName": "Advanced Web Programming",
      "semesterId": 2
    }
  ]
}
```

---

## 📘 Resource: Subjects (`/api/subjects`)

Deals with institutional syllabus/subjects.

### 1. Get List of Subjects
* **Method:** `GET`
* **Path:** `/api/subjects`

#### Example Response (`200 OK`):
```json
{
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 10,
    "totalPages": 1
  },
  "success": true,
  "message": "Request processed successfully",
  "data": [
    {
      "subjectId": 1,
      "subjectName": "Core C# Programming"
    }
  ]
}
```

---

### 2. Get Subject by ID
* **Method:** `GET`
* **Path:** `/api/subjects/{id}`

---

### 3. Create Subject
* **Method:** `POST`
* **Path:** `/api/subjects`
* **Request Body:**
  * `subjectName` (string, required): Name of the syllabus subject.

#### Example Request Body:
```json
{
  "subjectName": "Cloud Computing Fundamentals"
}
```

#### Example Response (`201 Created`):
```json
{
  "success": true,
  "message": "Tạo môn học thành công!",
  "data": {
    "subjectId": 11,
    "subjectName": "Cloud Computing Fundamentals"
  }
}
```

---

### 4. Update Subject
* **Method:** `PUT`
* **Path:** `/api/subjects/{id}`

---

### 5. Delete Subject
* **Method:** `DELETE`
* **Path:** `/api/subjects/{id}`

---

## ⚠️ Global Error Catalog & Status Codes

The LMS API respects proper HTTP response status codes. The following codes are standard responses when errors occur:

### `400 Bad Request`
Sent when model validation fails, or input payload data types are broken.
```json
{
  "success": false,
  "message": "Dữ liệu tạo mới không hợp lệ!",
  "errors": {
    "CourseName": [
      "The CourseName field is required."
    ]
  }
}
```

### `404 Not Found`
Sent when a path segment points to a resource ID that does not exist in the database.
```json
{
  "success": false,
  "message": "Course not found",
  "errors": "404 Not Found"
}
```

### `500 Internal Server Error`
Sent when an unexpected server error occurs (e.g., database connection down).
```json
{
  "success": false,
  "message": "An unexpected error occurred on the server.",
  "errors": "Internal Server Error"
}
```
