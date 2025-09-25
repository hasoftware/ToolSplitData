# Data Split Pro v1.0

## Mô tả

Data Split Pro là một công cụ đơn giản và hiệu quả để tách dữ liệu từ chuỗi văn bản thành các cột riêng biệt. Chương trình hỗ trợ nhiều định dạng delimiter khác nhau và có giao diện thân thiện với người dùng.

## Tính năng chính

- ✅ Tách dữ liệu theo delimiter tùy chỉnh (mặc định: `|`)
- ✅ Hỗ trợ nhiều dòng dữ liệu cùng lúc
- ✅ Hiển thị dữ liệu trong bảng có thể sắp xếp
- ✅ Copy dữ liệu đã tách vào clipboard
- ✅ Giao diện tối đơn giản và dễ sử dụng
- ✅ Hỗ trợ phím Enter để tách dữ liệu nhanh

## Cách sử dụng

1. Nhập dữ liệu cần tách vào ô "Nhập dữ liệu"
2. Chọn delimiter (mặc định là `|`)
3. Nhấn "Tách Dữ Liệu" hoặc Enter
4. Xem kết quả trong bảng bên dưới
5. Sử dụng "Copy" để sao chép dữ liệu

## Ví dụ

**Input:**

```
abc|bcd|uis|opp|opo
hello|world|test|data
```

**Output:**
| Column1 | Column2 | Column3 | Column4 | Column5 |
|---------|---------|---------|---------|---------|
| abc | bcd | uis | opp | opo |
| hello | world | test | data | |

## Yêu cầu hệ thống

- Windows 10/11
- .NET 6.0 Runtime

## Cài đặt

1. Tải file `.exe` từ releases
2. Chạy file cài đặt
3. Khởi động chương trình

## Build từ source code

```bash
dotnet build
dotnet run
```

## Thông tin Developer

- **Developer:** HoangAnhDev
- **Author:** HASOFTWARE
- **Contact:** @HoangAnhDev
- **Telegram Channel:** [@hasoftware](https://t.me/hasoftware)
- **Github:** [@hasoftware](https://github.com/hasoftware)

## License

Copyright © HASOFTWARE 2025. All rights reserved.
