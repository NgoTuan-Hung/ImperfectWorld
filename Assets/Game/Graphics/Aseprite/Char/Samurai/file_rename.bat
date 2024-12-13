@echo off
setlocal enabledelayedexpansion

:: Loop through all files in the current directory
for %%f in (*.*) do (
    :: Check if the file name contains "final_goku.Animations."
    set "filename=%%~nf"
    set "extension=%%~xf"
    
    if "!filename!" neq "!filename:samurai.Animations.=!" (
        :: Remove "final_goku.Animations." from the file name
        set "newname=!filename:samurai.Animations.=!"
        
        :: Rename the file
        ren "%%f" "!newname!!extension!"
        echo Renamed: %%f to !newname!!extension!
    )
)

endlocal
