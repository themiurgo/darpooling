using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Le informazioni generali relative a un assembly sono controllate dal seguente
// insieme di attributi. Per modificare le informazioni associate a un assembly
// occorre quindi modificare i valori di questi attributi.
[assembly: AssemblyTitle("UserNodeSmartDeviceGUI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("UserNodeSmartDeviceGUI")]
[assembly: AssemblyCopyright("Copyright ©  2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Se si imposta ComVisible su false, i tipi in questo assembly non saranno visibili
// ai componenti COM. Se è necessario accedere a un tipo in questo assembly da
// COM, impostare su true l'attributo ComVisible per tale tipo.
[assembly: ComVisible(false)]

// Se il progetto viene esposto a COM, il GUID che segue verrà utilizzato per creare l'ID typelib
[assembly: Guid("4159fbcd-f2eb-40dc-a73d-d29e0d50ea3c")]

// Le informazioni sulla versione di un assembly sono costituite dai seguenti quattro valori:
//
//      Numero di versione principale
//      Numero di versione secondario
//      Numero build
//      Revisione
//
[assembly: AssemblyVersion("1.0.0.0")]

// L'attributo seguente impedisce la visualizzazione dell'avviso FxCop "CA2232: Microsoft.Usage: Aggiungere STAThreadAttribute all'assembly"
// poiché l'applicazione dispositivo non supporta thread STA.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2232:MarkWindowsFormsEntryPointsWithStaThread")]
