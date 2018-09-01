using System.ComponentModel;

namespace Core.CmdLine
{
    /// <summary>
    /// Interface describing behavior of the class CmdLineObject
    /// </summary>
    public interface ICmdLineObject
	{
		/// <summary>
		/// Gets the options used for the handling the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		CmdLineOptions Options { get; }

		/// <summary>
		/// Gets or sets a value that determines if help should be displayed.
		/// </summary>
		[CmdLineArg(Alias = "?", ShowInUsage = DefaultBoolean.True)]
		[Description("Displays command-line usage information.")]
		[Browsable(false)]
		[DefaultValue(false)]
		bool Help { get; set; }

		/// <summary>
		/// Gets the list of command-line properties.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		CmdLinePropertyList Properties { get; }

		/// <summary>
		/// Gets the error text for the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		string ErrorText { get; }

		/// <summary>
		/// Gets a value that determines if the CmdLineObject is ready to use.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Gets the default properties for the command-line.
		/// </summary>
		CmdLineProperty[] DefaultProperties { get; }

		/// <summary>
		/// Initializes the CmdLineObject.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Initializes the command-line object, but does not populate it.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeEmpty();

		/// <summary>
		/// Initializes the command-line args based on a query string. Used for 
		/// </summary>
		/// <param name="queryStr"></param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeFromQueryString(string queryStr);

		/// <summary>
		/// Initializes the object with the given arguments.
		/// </summary>
		/// <param name="args">The command-line args. Make sure to shrink the array if the first element contains the path to the application (as in Environment.GetCommandLineArgs()) or the default parameter won't get set correctly.</param>
		/// <example>
		/// using BizArk.Core.ArrayExt;
		/// var args = Environment.GetCommandLineArgs().Shrink(1);
		/// </example>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeFromCmdLine(params string[] args);

		/// <summary>
		/// Makes sure the command-line object is valid. 
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		bool IsValid();

		/// <summary>
		/// Gets the full description for the command-line arguments.
		/// </summary>
		/// <param name="maxWidth">Determines the number of characters per line. Set this to Console.Width.</param>
		/// <returns></returns>
		string GetHelpText(int maxWidth);

		/// <summary>
		/// Saves the settings to an xml file.
		/// </summary>
		/// <param name="path"></param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void SaveToXml(string path);

		/// <summary>
		/// Restores the settings from an xml file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if the settings are restored from the file.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		bool RestoreFromXml(string path);

		/// <summary>
		/// Gets the usage for this command-line object.
		/// </summary>
		/// <returns></returns>
		string ToString();
	}
}