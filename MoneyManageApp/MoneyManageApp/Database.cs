using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManageApp
{
    public static class Database
    {
        private static string connectionString = "Data Source=database.db;Version=3;";

        // Método para obtener la conexión
        public static SQLiteConnection GetConnection()
        {
            if (!File.Exists("database.db"))
            {
                SQLiteConnection.CreateFile("database.db");
            }
            return new SQLiteConnection(connectionString);
        }

        // Método para inicializar la base de datos (crear tablas si no existen)
        public static void InitializeDatabase()
        {
            using (var connection = GetConnection())
            {
                // Asegurarse de que la conexión esté abierta
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // SQL para crear las tablas si no existen
                string createClientesTable = @"
                CREATE TABLE IF NOT EXISTS Clientes (
                    CedulaRUC TEXT PRIMARY KEY,
                    NombreRazonSocial TEXT NOT NULL,
                    Direccion TEXT,
                    Ciudad TEXT,
                    Telefono TEXT,
                    CorreoElectronico TEXT,
                    AnioNacimiento TEXT NOT NULL
                );";

                string createIngresosEgresosTable = @"
                CREATE TABLE IF NOT EXISTS IngresosEgresos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Fecha TEXT NOT NULL,
                    Concepto TEXT NOT NULL,
                    Ingresos REAL DEFAULT 0,
                    Egresos REAL DEFAULT 0,
                    Saldo REAL
                );";

                string createProductosTable = @"
                CREATE TABLE IF NOT EXISTS Productos (
                    Codigo TEXT PRIMARY KEY,
                    Articulo TEXT NOT NULL,
                    Entradas INTEGER DEFAULT 0,
                    Salidas INTEGER DEFAULT 0,
                    Stock INTEGER DEFAULT 0
                );";

                string createEntradasTable = @"
                CREATE TABLE IF NOT EXISTS Entradas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Codigo TEXT NOT NULL,
                    Articulo TEXT NOT NULL,
                    Fecha TEXT NOT NULL,
                    Cantidad INTEGER NOT NULL,
                    FOREIGN KEY(Codigo) REFERENCES Productos(Codigo)
                );";

                string createSalidasTable = @"
                CREATE TABLE IF NOT EXISTS Salidas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Codigo TEXT NOT NULL,
                    Articulo TEXT NOT NULL,
                    Fecha TEXT NOT NULL,
                    Cantidad INTEGER NOT NULL,
                    FOREIGN KEY(Codigo) REFERENCES Productos(Codigo)
                );";

                string createCuentasPorCobrarTable = @"
                CREATE TABLE IF NOT EXISTS CuentasPorCobrar (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Cliente TEXT NOT NULL,
                    Articulo TEXT NOT NULL,
                    ValorCredito REAL NOT NULL,
                    FechaInicial TEXT NOT NULL,
                    SaldoCuenta REAL NOT NULL,
                    FechaFinal TEXT,
                    Estado TEXT
                );";

                string createRecaudosDiariosTable = @"
                CREATE TABLE IF NOT EXISTS RecaudosDiarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RecaudoDiario REAL NOT NULL,
                    FechaCobro TEXT NOT NULL,
                    NombreCliente TEXT NOT NULL,
                    Concepto TEXT NOT NULL
                );";

                string createCitasTable = @"
                CREATE TABLE IF NOT EXISTS Citas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Cliente TEXT NOT NULL,
                    Fecha TEXT NOT NULL,
                    Hora TIME NOT NULL,
                    Estado TEXT NOT NULL,
                    Concepto TEXT NOT NULL
                );";

                string createOdontogramasTable = @"
                CREATE TABLE IF NOT EXISTS Odontogramas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClienteId TEXT NOT NULL,
                    FechaRegistro TEXT NOT NULL,
                    DoctorId TEXT NULL,
                    Especificaciones TEXT,
                    Observaciones TEXT,
                    DientesEstado TEXT NOT NULL, -- JSON string containing teeth states
                    FOREIGN KEY(ClienteId) REFERENCES Clientes(CedulaRUC)
                );";
                string createFotosTable = @"
                 CREATE TABLE IF NOT EXISTS Fotos (
                       Id INTEGER PRIMARY KEY AUTOINCREMENT,
                       CedulaRUC TEXT NOT NULL,
                       Foto BLOB NOT NULL,  -- La foto se almacenará como BLOB
                       FechaSubida TEXT NOT NULL,
                       FOREIGN KEY(CedulaRUC) REFERENCES Clientes(CedulaRUC)
                );";
             


                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createClientesTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createIngresosEgresosTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createProductosTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createEntradasTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createSalidasTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createCuentasPorCobrarTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createRecaudosDiariosTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createOdontogramasTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createCitasTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createFotosTable;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
