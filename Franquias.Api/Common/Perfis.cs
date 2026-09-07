namespace Franquias.Api.Common
{
    // Nomes dos perfis usados nos atributos de autorizacao.
    public static class Perfis
    {
        public const string Administrador = "AdministradorFranqueadora";
        public const string Gestor = "GestorUnidade";
        public const string Operador = "Operador";

        public const string AdministradorOuGestor = Administrador + "," + Gestor;
        public const string Todos = Administrador + "," + Gestor + "," + Operador;
    }
}
