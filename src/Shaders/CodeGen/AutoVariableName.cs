/* Author:  Leonardo Trevisan Silio
 * Date:    02/09/2024
 */
namespace Radiance.Shaders.CodeGen;

/// <summary>
/// Used to manage and generate automatically variable names.
/// </summary>
public static class AutoVariableName
{
    static int variableCount = 0;
    public static string Next(string type, int count = 4)
    {
        variableCount++;
        var varName = 
            type.Length > count ?
            type.Substring(0, count) :
            type;
        return $"{varName}{variableCount}";
    }
}