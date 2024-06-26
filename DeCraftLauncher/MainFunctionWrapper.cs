﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.UIControls.Popup;
using DeCraftLauncher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DeCraftLauncher
{
    public class MainFunctionWrapper
    {
        public static void LaunchMainFunctionWrapper(string className, MainWindow caller, JarConfig jar)
        {
            bool isDefaultPackage = !className.Contains(".");

            MainWindow.EnsureDir("./java_temp");
            File.WriteAllText("./java_temp/MainFunctionWrapper.java", JavaCode.GenerateMainFunctionWrapperCode(className, jar, isDefaultPackage));
            if (jar.appletEmulateHTTP)
            {
                File.WriteAllText("./java_temp/InjectedStreamHandlerFactory.java", JavaCode.GenerateHTTPStreamInjectorCode(jar, isDefaultPackage));
            }
            List<string> compilerOut;
            try
            {
                compilerOut = JarUtils.RunProcessAndGetOutput(MainWindow.mainRTConfig.javaHome + "javac", $"-cp \"{MainWindow.jarDir}/{jar.jarFileName}\" " +
                    $"./java_temp/MainFunctionWrapper.java " +
                    (jar.appletEmulateHTTP ? $"./java_temp/InjectedStreamHandlerFactory.java " : "") +
                    $"-d ./java_temp " +
                    (jar.appletEmulateHTTP && MainWindow.mainRTConfig.isJava9 ? "--add-exports java.base/sun.net.www.protocol.http=ALL-UNNAMED " : ""), true);
            }
            catch (ApplicationException)
            {
                PopupOK.ShowNewPopup("Failed to compile the Main function Wrapper.\n\nNote: the Main function Wrapper only supports JDK 6+", "DECRAFT");
                return;
            }
            Console.WriteLine("Compilation log:");
            foreach (string a in compilerOut)
            {
                Console.WriteLine(a);
            }


            //launch
            JavaExec mainFunctionExec = new JavaExec(!isDefaultPackage ? "decraft_internal.MainFunctionWrapper" : "MainFunctionWrapper");

            mainFunctionExec.classPath.Add(Path.GetFullPath("./java_temp/"));
            mainFunctionExec.classPath.Add(Path.GetFullPath(MainWindow.jarDir + "/" + jar.jarFileName));
            if (jar.LWJGLVersion != "+ built-in")
            {
                mainFunctionExec.classPath.Add($"{MainWindow.currentDirectory}/lwjgl/{jar.LWJGLVersion}/*");
            }
            mainFunctionExec.jvmArgs.Add($"-Djava.library.path=\"{MainWindow.currentDirectory}/lwjgl/{(jar.LWJGLVersion == "+ built-in" ? "_temp_builtin" : jar.LWJGLVersion)}/native\"");
            mainFunctionExec.jvmArgs.Add(jar.jvmArgs);
            if (jar.appletEmulateHTTP && MainWindow.mainRTConfig.isJava9)
            {
                mainFunctionExec.jvmArgs.Add("--add-exports java.base/sun.net.www.protocol.http=ALL-UNNAMED");
            }
            mainFunctionExec.programArgs.Add($"\"{jar.playerName}\"");
            if (jar.server_ip != "")
            {
                mainFunctionExec.programArgs.Add($"--server {jar.server_ip.Replace(":", " --port ")}");
            }
            mainFunctionExec.programArgs.Add(jar.sessionID);
            mainFunctionExec.programArgs.Add(jar.gameArgs);
            Console.WriteLine("Running command: java " + mainFunctionExec.GetFullArgsString());

            string emulatedAppDataDir = Path.GetFullPath($"{MainWindow.currentDirectory}/{MainWindow.instanceDir}/{jar.instanceDirName}");
            mainFunctionExec.appdataDir = emulatedAppDataDir;
            mainFunctionExec.workingDirectory = $"{emulatedAppDataDir}{(jar.cwdIsDotMinecraft ? "/.minecraft" : "")}";
            try
            {
                mainFunctionExec.StartOpenWindowAndAddToInstances(caller, jar);
            }
            catch (Win32Exception w32e)
            {
                PopupOK.ShowNewPopup($"Error launching java process: {w32e.Message}\n\nVerify that Java is installed in \"Runtime settings\".");
            }
        }
    }
}
