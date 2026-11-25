public static class ImageSniffer
{
	/// Returns the content type detected from the file's magic bytes, or null if unsupported.
	public static string? Detect(ReadOnlySpan<byte> data)
	{
		if (
			data.Length >= 8
			&& data[..8]
				.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })
		)
			return "image/png";

		if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
			return "image/jpeg";
		return null;
	}
}
