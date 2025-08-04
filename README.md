# README - Sistema de Biblioteca Digital

## 📖 Visão Geral
Sistema de gerenciamento de biblioteca digital com ASP.NET Core e Entity Framework.

## 🛠️ Pré-requisitos
- .NET 6.0+
- SQL Server Express LocalDB (já vem com Visual Studio)
- Visual Studio 2022 ou VS Code

## 🚀 Configuração Rápida
1. Clone o repositório
2. Configure a conexão no `appsettings.json`:
```json
"ConnectionStrings": {
  "BibliotecaContext": "Server=(localdb)\\mssqllocaldb;Database=Biblioteca;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. Execute no terminal:
```bash
dotnet ef database update
dotnet run
```

## 📦 Exportar Banco LocalDB

### Método 1: Usando SSMS
```bash
sqlpackage /Action:Export /SourceServerName:(localdb)\mssqllocaldb /SourceDatabase:Biblioteca /TargetFile:Biblioteca.bacpac
```

### Método 2: Script SQL
```bash
sqlcmd -S (localdb)\mssqllocaldb -d Biblioteca -Q "BACKUP DATABASE [Biblioteca] TO DISK='C:\backup\Biblioteca.bak'"
```

## 🔄 Importar para LocalDB
```bash
sqlpackage /Action:Import /SourceFile:Biblioteca.bacpac /TargetServerName:(localdb)\mssqllocaldb /TargetDatabaseName:Biblioteca
```

## ⚠️ Dicas Importantes
1. Verifique se o LocalDB está instalado e rodando:
```bash
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

2. Para visualizar seus bancos LocalDB:
```bash
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT name FROM sys.databases"
```

3. Se encontrar erros de conexão, tente:
```bash
sqllocaldb stop MSSQLLocalDB
sqllocaldb delete MSSQLLocalDB
sqllocaldb create MSSQLLocalDB
```

## 📊 Gerenciamento Básico
- **Listar tabelas**:
```bash
sqlcmd -S (localdb)\mssqllocaldb -d Biblioteca -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

- **Fazer backup rápido**:
```bash
sqlcmd -S (localdb)\mssqllocaldb -d Biblioteca -Q "BACKUP DATABASE [Biblioteca] TO DISK='C:\temp\Biblioteca.bak' WITH FORMAT"
```
