# Folder Merging

Moves all album folders with the same artist name in a single parent folder named after the artist.

This is how I organize my album folders and I got tired of doing to by hand even with macros.

If half of the folders inside are already organized, this will still properly organize the rest.

## Arguments
Pass in the path to the parent directory where all of the album folders are located.

Example: `FolderMerging.exe "C:\Music Folder\New Albums"`

Pass -f to force skipping Press Any Key prompts for non-error situations. Errors will always have Press Any Key

## Assumptions
Album folders must be in the format of "artist name - album name' in order for this to work properly.
