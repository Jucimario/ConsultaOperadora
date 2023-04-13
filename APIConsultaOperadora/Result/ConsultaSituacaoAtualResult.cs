namespace APIConsultaOperadora.Result;

public class ConsultaSituacaoAtualResult
{
    public class ConsultaSituacaoAtual : BaseResult
    {
        public string? TipoTelefone { get; set; }
        public string? Portabilidade { get; set; }
        public string? Prestadora { get; set; }
        public string? Estado { get; set; }
        public string? Cidade { get; set; }
    }
}
