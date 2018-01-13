# Taiko_RE
a pretty horrible attempt at reverse engineering the taiko no tatsujin note sheet format

I started this back in 2013 and lost interest because, effectively, I didn't know what a float looked like in big endian hex. Now I do. It took a very long time to work out that 0x3F800000 actually meant something, let alone something important like "1.0".

me smort

guesswork.txt contains a whole lot of random chunks and bars from different songs, as a way to help me work out how these files are thrown together.
Right now this uses some very crappy methods to get the data; it's using regex and indexed lists to go through each of the 32 bit chunks of data as strings, then converts them to floats and ints and such. it's bad.


As it stands, this *should* be able to read *most* of the data from the sheets in Wii4/5, and DX on PSP. The DLC files in DX definitely seem to use the same file structure. I haven't checked the ISO for the actual game, though. Presumably it's the same.