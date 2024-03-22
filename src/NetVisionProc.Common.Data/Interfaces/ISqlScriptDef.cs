namespace NetVisionProc.Common.Data.Interfaces
{
    public interface ISqlScriptDef
    {
        string GetSql(bool isSqLite = false);
    }
}