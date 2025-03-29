using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceshipArcade.MG.Engine.Utilities;

public enum RootPath
{
	AppData,
	AssetsFolder,
	Content,
	Source,
}

public static class FileManager
{
	// Basic Paths where relevant files are
	public static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tilteroids");
	public static readonly string AssetsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
	public static readonly string SourceFolder = "C:\\Projects\\Tilteroids\\Tilteroids.Main";

	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

	// Public functions
	public static bool FileExists(RootPath rootPath, string relativePath)
	{
		string path = rootPath switch
		{
			RootPath.AppData => Path.Combine(AppDataFolder, relativePath),
			_ => relativePath
		};

		return File.Exists(path);
	}

	public static T LoadJson<T>(RootPath rootPath, string relativePath)
	{
		string json = LoadTextFile(rootPath, relativePath);
		return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidDataException("JsonConvert.DeserializeObject returned a null.");
	}
	public static void SaveJson(RootPath rootPath, string relativePath, object value)
	{
		string json = JsonSerializer.Serialize(value, JsonOptions);
		SaveTextFile(rootPath, relativePath, json);
	}

	public static Texture2D LoadImage(RootPath rootPath, string relativePath, GraphicsDevice device)
	{
		return rootPath switch
		{
			RootPath.AssetsFolder => Texture2D.FromFile(device, Path.Combine(AssetsFolder, relativePath)),
			RootPath.Source => Texture2D.FromFile(device, Path.Combine(SourceFolder, relativePath)),
			_ => throw new NotImplementedException($"File source {rootPath} not implemented for {nameof(LoadImage)}"),
		};
	}
	public static void SaveImage(RootPath rootPath, string relativePath, Texture2D texture)
	{
		switch (rootPath)
		{
			case RootPath.AssetsFolder: SaveToFolder(AssetsFolder); break;
			case RootPath.AppData: SaveToFolder(AppDataFolder); break;
			case RootPath.Source: SaveToFolder(SourceFolder); break;
			default: throw new NotImplementedException($"File source {rootPath} not implemented for {nameof(SaveImage)}");
		}

		void SaveToFolder(string fileSource)
		{
			string path = Path.GetDirectoryName(relativePath) ?? "";
			string file = Path.GetFileName(relativePath);
			path = Path.Combine(fileSource, path);

			// If directory doesn't exist, create it
			Directory.CreateDirectory(path);
			using var stream = File.OpenWrite(Path.Combine(path, file));
			texture.SaveAsPng(stream, texture.Width, texture.Height);
		}
	}

	// Private functions
	private static string LoadTextFile(RootPath rootPath, string relativePath)
	{
		return rootPath switch
		{
			RootPath.AssetsFolder => LoadFromFolder(AssetsFolder),
			RootPath.AppData => LoadFromFolder(AppDataFolder),
			RootPath.Content => LoadFromContent(),
			RootPath.Source => LoadFromFolder(SourceFolder),
			_ => throw new NotImplementedException($"File source {rootPath} not implemented"),
		};

		string LoadFromFolder(string fileSource)
		{
			string path = Path.Combine(fileSource, relativePath);
			return File.ReadAllText(path);
		}
		string LoadFromContent()
		{
			string path = Path.Combine("Content", relativePath);
			using var stream = TitleContainer.OpenStream(path);
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
	private static void SaveTextFile(RootPath rootPath, string relativePath, string content)
	{
		switch (rootPath)
		{
			case RootPath.AssetsFolder: SaveToFolder(AssetsFolder); break;
			case RootPath.AppData: SaveToFolder(AppDataFolder); break;
			case RootPath.Source: SaveToFolder(SourceFolder); break;
			case RootPath.Content: SaveToContent(); break;
			default: throw new NotImplementedException($"File source {rootPath} not implemented");
		};

		void SaveToFolder(string fileSource)
		{
			string path = Path.GetDirectoryName(relativePath) ?? "";
			string file = Path.GetFileName(relativePath);
			path = Path.Combine(fileSource, path);

			// If directory doesn't exist, create it
			Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, file), content);
		}
		void SaveToContent()
		{
			// TODO: WARNING! UNTESTED!
			string path = Path.Combine("Content", relativePath);
			using var stream = TitleContainer.OpenStream(path);
			using var writer = new StreamWriter(stream);
			writer.WriteAsync(content);
		}
	}
}