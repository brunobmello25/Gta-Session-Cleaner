using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TesteGta
{
    class GerenciadorProcesso
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);


        private static void SuspenderProcesso(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        private static void ResumirProcesso(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }

        private static Process[] EncontrarIdProcesso(string nome)
        {
            return Process.GetProcessesByName(nome);
        }

        public static void Executar()
        {
            Console.WriteLine("Feito por: Bruno Barros Mello");
            Console.WriteLine("-----------------------------\n");

            var processos = EncontrarIdProcesso("GTA5");

            if (processos.Length >= 1)
            {
                Console.WriteLine("Suspendendo Processo...");
                foreach (var processo in processos)
                {
                    SuspenderProcesso(processo.Id);
                }
                System.Threading.Thread.Sleep(11000);

                Console.WriteLine("Resumindo Processo...");
                foreach (var processo in processos)
                {
                    ResumirProcesso(processo.Id);
                }

                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("ERRO: Não foi encontrado nenhum processo de GTA5 em execução");
                Console.WriteLine("\nAperte qualquer tecla para continuar");
                Console.ReadKey();
            }
        }
    }
}
