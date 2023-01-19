using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Game
{
    public class PrintAllPlayerLoopSystems
    {
        [RuntimeInitializeOnLoadMethod]
        private static void ApplicationStart()
        {
            var defaultPlayerLoop = PlayerLoop.GetDefaultPlayerLoop();
            var stringBuilder = new StringBuilder();
            PrintPlayerLoopSystemRecursive(in defaultPlayerLoop, stringBuilder, 0);
            Debug.unityLogger.Log(nameof(PrintAllPlayerLoopSystems), stringBuilder.ToString());
        }

        private static void PrintPlayerLoopSystemRecursive(in PlayerLoopSystem system, StringBuilder stringBuilder, int depth)
        {
            if (depth == 0)
            {
                stringBuilder.AppendLine("ROOT NODE");
            }
            else if (system.type != null)
            {
                stringBuilder.Append('\t', depth);
                stringBuilder.AppendLine(system.type.Name);
            }

            if (system.subSystemList != null)
            {
                depth += 1;
                foreach (var subSystem in system.subSystemList)
                {
                    PrintPlayerLoopSystemRecursive(in subSystem, stringBuilder, depth);
                }
            }
        }
    }
}