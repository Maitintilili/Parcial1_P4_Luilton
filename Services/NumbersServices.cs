using Dapper;
using Microsoft.Data.Sqlite;
using Primer_Parcial_Luilton.Models;

namespace Primer_Parcial_Luilton.Services;

public class NumbersService(IConfiguration config)
{
    private readonly string _cs = config.GetConnectionString("DefaultConnection")!;

    private SqliteConnection Conn() => new(_cs);

    private static NumberRecordGet ToGet(dynamic r) =>
        new((int)r.Id, DateTime.Parse((string)r.Fecha), (int)r.Numero, (int)r.Resultado);

    public async Task InicializeAsync()
    {
        using var c = Conn();
        await c.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS NumberRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Fecha TEXT NOT NULL,
                Numero INTEGER NOT NULL,
                Resultado INTEGER NOT NULL)");
    }

    public async Task<NumberRecordGet> SaveAsync(NumberRecordSet r)
    {
        var resultado = r.Numero * r.Numero;
        using var c = Conn();
        var fecha = DateTime.Now;
        var Id = await c.ExecuteScalarAsync<int>(
            @"INSERT INTO NumberRecords (Fecha, Numero, Resultado)
            VALUES (@Fecha, @Numero, @Resultado);
            SELECT last_insert_rowid();",
            new { Fecha = fecha.ToString("o"), r.Numero, Resultado = resultado});
        return new NumberRecordGet(Id, fecha, r.Numero, resultado);
    }

    public async Task<bool> UpdateAsync(int id,NumberRecordSet r)
    {
        var resultado = r.Numero * r.Numero;
        using var c = Conn();
        var filas = await c.ExecuteAsync(
            @"UPDATE NumberRecords
            SET Fecha=@Fecha, Numero=@Numero, Resultado=@Resultado
            WHERE Id=@Id",
            new { Id = id, Fecha = DateTime.Now.ToString("o"), r.Numero, Resultado = resultado });
        return filas > 0;
    }

    public async Task<NumberRecordGet?> GetByIdAsync(int id)
    {
        using var c = Conn();
        var fila = await c.QuerySingleOrDefaultAsync(
            "SELECT * FROM NumberRecords WHERE Id=@id", new { id });
        return fila is null ? null : ToGet(fila);
    }
    
    public async Task<List<NumberRecordGet>> GetListAsync()
    {
        using var c = Conn();
        var filas = await c.QueryAsync("SELECT * FROM NumberRecords ORDER BY Id DESC");
        return filas.Select(f => (NumberRecordGet)ToGet(f)).ToList();
    }
}
