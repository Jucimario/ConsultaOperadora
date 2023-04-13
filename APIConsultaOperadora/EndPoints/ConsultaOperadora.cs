namespace APIConsultaOperadora.EndPoints;

public static class ConsultaOperadora
{

    public static void MapConsultaOperadora(this WebApplication app)
    {
        app.MapGet("/consultaoperadora", (string numeroTelefone) =>
        {
            var _dados = "conta efetuada aqui";

            if (string.IsNullOrEmpty(_dados))
                return Results.NotFound();

            return Results.Ok(_dados);
        }).WithName("GetConsultaOperadora"); ;
    }


}
