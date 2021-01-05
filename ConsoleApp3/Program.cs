using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestProject;
using OpCode = System.Reflection.Emit.OpCode;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            // var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            // var testProjectAssemblyPath = Path.Combine(Path.GetDirectoryName(executingAssemblyPath), "TestProject.dll");
            //
            // using var assembly = AssemblyDefinition.ReadAssembly(testProjectAssemblyPath);
            //
            // var testMethodsType = assembly.MainModule.Types.FirstOrDefault(t => t.Name == nameof(TestMethods));
            //
            // foreach (var method in testMethodsType.Methods) {
            //     var dynamicMethod = CreateDynamicMethod(method);
            //     var testMethodsInstance = new TestMethods();
            //     
            //     dynamicMethod.Invoke(testMethodsInstance, null);
            // }
            
            CreateDynamicMethod();
        }

        public delegate void WriteLineDelegate(string s);
        
        private static DynamicMethod CreateDynamicMethod()
        {
            var dynamicMethod = new DynamicMethod("DynamicMethod", null, null); // supply return type and parameter types 
            var dynamicIlInfo = dynamicMethod.GetDynamicILInfo();
            //Console.WriteLine("Hello, world");

            var writeLineMethodInfo = typeof(Console).GetMethods().FirstOrDefault(m => m.Name == "WriteLine" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string));

            var methodBody = writeLineMethodInfo.GetMethodBody();
            var buffer = methodBody.GetILAsByteArray();
            
            dynamicIlInfo.SetCode(buffer, methodBody.MaxStackSize);

            var del = (WriteLineDelegate)dynamicMethod.CreateDelegate(typeof(WriteLineDelegate));
            del("Hello, world");
            
            var helloWorldToken = dynamicIlInfo.GetTokenFor("Hello, world!");
            var writeLineToken = dynamicIlInfo.GetTokenFor(writeLineMethodInfo.MethodHandle);
            
            var instructions = new[] {
                new IlInstruction(System.Reflection.Emit.OpCodes.Ldstr, helloWorldToken, 0),
                new IlInstruction(System.Reflection.Emit.OpCodes.Call, writeLineToken, 1),
                new IlInstruction(System.Reflection.Emit.OpCodes.Ret, null, 2),
            };

            using var ms = new MemoryStream();
            return null;
            //
            // new BinaryWriter()
            // dynamicIlInfo.SetCode();
            // //dynamicIlInfo.SetExceptions();
            // //dynamicIlInfo.SetLocalSignature();
            //
            // return dynamicMethod.CreateDelegate();
        }

        string A(string s){
            return s?.ToLower();
        }
    }
    
    public class IlInstruction
    {
        public OpCode OpCode { get; set; }
        public object Operand { get; set; }
        public int Index { get; }
        public IlInstruction(OpCode opCode, object operand, int index)
        {
            OpCode = opCode;
            Operand = operand;
            Index = index;
        }

        public override string ToString()
        {
            return $"{Index}: {OpCode} {Operand}";
        }
    }
}