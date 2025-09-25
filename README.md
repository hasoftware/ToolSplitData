# Data Split Pro v1.0

<div align="center">

![Data Split Pro](https://img.shields.io/badge/Data%20Split%20Pro-v1.0-blue?style=for-the-badge&logo=dotnet)
![.NET](https://img.shields.io/badge/.NET-6.0-purple?style=for-the-badge&logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows-lightblue?style=for-the-badge&logo=windows)
![License](https://img.shields.io/badge/License-Copyright%20HASOFTWARE-red?style=for-the-badge)

**Một công cụ mạnh mẽ và dễ sử dụng để tách dữ liệu văn bản thành các cột có cấu trúc**

[📥 Download](https://github.com/hasoftware/ToolSplitData/releases) • [📖 Documentation](#-hướng-dẫn-sử-dụng) • [🐛 Report Bug](https://github.com/hasoftware/ToolSplitData/issues) • [💡 Request Feature](https://github.com/hasoftware/ToolSplitData/issues)

</div>

---

## 📋 Tổng quan

**Data Split Pro** là một ứng dụng Windows Forms được phát triển bằng C# và .NET 6.0, được thiết kế để giải quyết nhu cầu tách dữ liệu văn bản một cách nhanh chóng và chính xác. Ứng dụng cung cấp giao diện trực quan, dễ sử dụng và hỗ trợ nhiều định dạng delimiter khác nhau.

### 🎯 Vấn đề giải quyết

- **Xử lý dữ liệu thô**: Chuyển đổi dữ liệu văn bản không có cấu trúc thành bảng có tổ chức
- **Tiết kiệm thời gian**: Tự động hóa quá trình tách dữ liệu thủ công
- **Tăng độ chính xác**: Giảm thiểu lỗi trong quá trình xử lý dữ liệu
- **Hỗ trợ đa định dạng**: Làm việc với nhiều loại delimiter khác nhau

## ✨ Tính năng nổi bật

### 🔧 Chức năng cốt lõi
- **Tách dữ liệu thông minh**: Hỗ trợ delimiter tùy chỉnh (mặc định: `|`)
- **Xử lý đa dòng**: Tách nhiều dòng dữ liệu cùng lúc
- **Hiển thị trực quan**: Bảng dữ liệu có thể sắp xếp và tương tác
- **Sao chép nhanh**: Copy dữ liệu đã xử lý vào clipboard

### 🚀 Trải nghiệm người dùng
- **Giao diện hiện đại**: Thiết kế tối giản, thân thiện
- **Phím tắt**: Hỗ trợ phím Enter để thao tác nhanh
- **Xử lý lỗi**: Thông báo rõ ràng khi có lỗi xảy ra
- **Tùy chỉnh linh hoạt**: Cấu hình delimiter theo nhu cầu

### 📊 Khả năng xử lý
- **Hiệu suất cao**: Xử lý hàng nghìn dòng dữ liệu
- **Bộ nhớ tối ưu**: Quản lý tài nguyên hiệu quả
- **Tương thích tốt**: Chạy ổn định trên Windows 10/11

## 🖼️ Giao diện ứng dụng

```
┌─────────────────────────────────────────────────────────────┐
│                    Data Split Pro v1.0                     │
├─────────────────────────────────────────────────────────────┤
│ Nhập dữ liệu: [abc|bcd|uis|opp|opo                    ]    │
│ Delimiter: [|] [Tách Dữ Liệu] [Clear] [Copy]              │
├─────────────────────────────────────────────────────────────┤
│ Column1 │ Column2 │ Column3 │ Column4 │ Column5            │
├─────────┼─────────┼─────────┼─────────┼─────────┤
│ abc     │ bcd     │ uis     │ opp     │ opo     │
│ hello   │ world   │ test    │ data    │         │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Hướng dẫn sử dụng

### Cài đặt nhanh

1. **Tải xuống**: Truy cập [Releases](https://github.com/hasoftware/ToolSplitData/releases)
2. **Chạy ứng dụng**: Double-click file `DataSplitPro.exe`
3. **Bắt đầu sử dụng**: Nhập dữ liệu và nhấn Enter

### Hướng dẫn chi tiết

#### Bước 1: Nhập dữ liệu
- Mở ứng dụng Data Split Pro
- Nhập dữ liệu cần tách vào ô "Nhập dữ liệu"
- Hỗ trợ nhiều dòng dữ liệu cùng lúc

#### Bước 2: Cấu hình delimiter
- Chọn delimiter phù hợp (mặc định: `|`)
- Các delimiter phổ biến: `,`, `;`, `\t`, ` `, `-`, `_`

#### Bước 3: Tách dữ liệu
- Nhấn nút "Tách Dữ Liệu" hoặc phím Enter
- Xem kết quả trong bảng bên dưới

#### Bước 4: Xuất dữ liệu
- Sử dụng nút "Copy" để sao chép vào clipboard
- Dán vào Excel, Word hoặc ứng dụng khác

## 💡 Ví dụ thực tế

### Ví dụ 1: Dữ liệu CSV
**Input:**
```
John,Doe,25,Developer,New York
Jane,Smith,30,Designer,Los Angeles
Bob,Johnson,28,Manager,Chicago
```

**Output:**
| First Name | Last Name | Age | Position | City |
|------------|-----------|-----|----------|------|
| John | Doe | 25 | Developer | New York |
| Jane | Smith | 30 | Designer | Los Angeles |
| Bob | Johnson | 28 | Manager | Chicago |

### Ví dụ 2: Dữ liệu pipe-separated
**Input:**
```
Product A|$19.99|In Stock|Electronics
Product B|$29.99|Out of Stock|Clothing
Product C|$9.99|In Stock|Books
```

**Output:**
| Product | Price | Status | Category |
|---------|-------|--------|----------|
| Product A | $19.99 | In Stock | Electronics |
| Product B | $29.99 | Out of Stock | Clothing |
| Product C | $9.99 | In Stock | Books |

## 🛠️ Yêu cầu hệ thống

| Thành phần | Yêu cầu |
|------------|---------|
| **Hệ điều hành** | Windows 10 (version 1903+) / Windows 11 |
| **.NET Runtime** | .NET 6.0 Desktop Runtime |
| **RAM** | Tối thiểu 512MB, khuyến nghị 1GB+ |
| **Dung lượng** | ~50MB cho ứng dụng |
| **Màn hình** | Độ phân giải tối thiểu 1024x768 |

## 🔨 Build từ source code

### Yêu cầu phát triển
- Visual Studio 2022 hoặc VS Code
- .NET 6.0 SDK
- Windows 10/11

### Các bước build

```bash
# Clone repository
git clone https://github.com/hasoftware/ToolSplitData.git
cd ToolSplitData

# Restore dependencies
dotnet restore

# Build project
dotnet build --configuration Release

# Run application
dotnet run
```

### Cấu trúc dự án
```
ToolSplitData/
├── DataSplitPro.csproj    # Project configuration
├── MainForm.cs            # Main application logic
├── Program.cs             # Application entry point
├── hasoftware.ico         # Application icon
├── README.md              # Documentation
└── .gitignore             # Git ignore rules
```

## 🤝 Đóng góp

Chúng tôi hoan nghênh mọi đóng góp từ cộng đồng! Hãy tham gia phát triển dự án:

1. **Fork** repository này
2. **Tạo branch** cho tính năng mới (`git checkout -b feature/AmazingFeature`)
3. **Commit** thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. **Push** lên branch (`git push origin feature/AmazingFeature`)
5. **Tạo Pull Request**

### Báo cáo lỗi
- Sử dụng [Issues](https://github.com/hasoftware/ToolSplitData/issues) để báo cáo lỗi
- Mô tả chi tiết vấn đề và cách tái tạo
- Đính kèm screenshot nếu cần thiết

## 📈 Roadmap

### Version 1.1 (Planned)
- [ ] Hỗ trợ import file CSV/Excel
- [ ] Export dữ liệu ra nhiều định dạng
- [ ] Tùy chỉnh giao diện (theme)
- [ ] Lưu cấu hình delimiter

### Version 1.2 (Future)
- [ ] Plugin system
- [ ] Batch processing
- [ ] Command line interface
- [ ] API integration

## 📞 Liên hệ & Hỗ trợ

<div align="center">

**HASOFTWARE Development Team**

| Platform | Link |
|----------|------|
| **GitHub** | [@hasoftware](https://github.com/hasoftware) |
| **Telegram** | [@hasoftware](https://t.me/hasoftware) |
| **Developer** | [@HoangAnhDev](https://github.com/HoangAnhDev) |

</div>

## 📄 License

```
Copyright © HASOFTWARE 2025. All rights reserved.

This software is proprietary and confidential. 
Unauthorized copying, distribution, or modification is strictly prohibited.
```

---

<div align="center">

**⭐ Nếu dự án hữu ích, hãy cho chúng tôi một star! ⭐**

Made with ❤️ by [HASOFTWARE](https://github.com/hasoftware)

</div>