# Giai đoạn 1: Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Sao chép file solution và tất cả các file project
COPY ["ClassManager.sln", "."]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Api/Api.csproj", "src/Api/"]

# Chạy restore trên toàn bộ solution.
RUN dotnet restore "ClassManager.sln"

# Sau khi restore xong, sao chép toàn bộ source code còn lại.
COPY . .

# Chuyển vào thư mục của project Api và thực hiện publish
WORKDIR "/source/src/Api"
# XÓA BỎ CỜ --no-restore ĐỂ LỆNH PUBLISH ỔN ĐỊNH HƠN
RUN dotnet publish -c Release -o /app/publish

# Giai đoạn 2: Tạo image cuối cùng
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Api.dll"]