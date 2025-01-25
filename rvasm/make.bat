@echo off

set "texteditor=C:\Users\Dell\Desktop\instalki\notepad++.exe"

set "gcc=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-gcc.exe"
set "gpp=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-g++.exe"
set "asm=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-as.exe"
set "link=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-ld.exe"
set "disasm=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-objdump.exe"
set "objcopy=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-objcopy.exe"
set "addr2line=C:\SysGCC\risc-v\bin\riscv64-unknown-elf-addr2line.exe"

set "ldscript=simlink.ld"
set "ldargs"=--export-dynamic --no-strip-discarded" 
set "ldarch=-A riscv:rv32im"
set "arch=-mno-relax -march=rv32im -mabi=ilp32" 
: set "arch=-mno-relax -march=rv32im -mabi=ilp32 -DPERFORMANCE=1"
set "emulation=-m elf32lriscv"
set "disasmopt=-D -M numeric,no-aliases --visualize-jumps"
set "addr2lineopt=--addresses --functions --inlines --pretty-print --basenames --target=elf32-littleriscv --demangle=gnat"
set "floatlib=--library-path lib\RVfplib\build\lib --library rvfp"

set "src=%1"
set "obj=%~n1.o"
set "crt0=system\crt0.s"
set "crt0obj=crt0.o"
set "boot=system\boot.c"
set "bootobj=boot.o"
set "ldout=%~n1.elf"
set "binoutdir=%~dp0\..\rvbin\%~n1\"	
set "optimlvl="

setlocal

if "%src%" == ""  ( goto :help )

set "optemp=%2"
if "%optemp:~0,2%" == "-O" (
    set "optimlvl=%2"
)

for %%i in (%*) do (
    if "%%i" == "--help" ( 
        set "arg_help=1"
    ) else if "%%i" == "--noboot" ( 
        set "arg_noboot=1" 
    ) else if "%%i" == "--nodasm" ( 
        set "arg_nodasm=1" 
    ) else if "%%i" == "--nodata" ( 
        set "arg_nodata=1" 
    ) else if "%%i" == "--nclean" ( 
        set "arg_nclean=1" 
    ) else if "%%i" == "--nshow" ( 
        set "arg_nshow=1" 
    ) else if "%%i" == "--outelf" ( 
        set "arg_outelf=1" 
    ) else if "%%i" == "--fplib" ( 
        set "arg_fplib=1" 
    ) else if not %%i == %src% (
        if optimlvl == "" ( 
            set "invalidarg=%%i"
        ) 
    )
)
if defined invalidarg ( goto :argunknown )
if defined arg_help ( goto :help )

echo:
echo Compiling %src% to little-endian objects...
if defined optimlvl (
    echo Optimization level: %optimlvl%
)

set "optionalldargs="
if defined arg_fplib (
    set "optionalldargs=%floatlib%"
    echo ^(Using "RVfplib" floating-point library^)
)

if "%src:~-2%" == ".c" (
    set "compiler=%gcc%"
)
if "%src:~-4%" == ".cpp" (
    set "compiler=%gpp%"
)

: if given src is .c file
if defined compiler (
    echo Compiling %src% with %compiler%...

    %compiler% %optimlvl% -c %src% -o %obj% %arch% -ffreestanding -nostdlib
    if %errorlevel% neq 0 ( goto :FAIL )
    if not defined arg_noboot (
        echo Compiling %boot% %crt0% and linking to %src%...
        %compiler% %optimlvl% -c %boot% -o %bootobj% %arch% -ffreestanding -nostdlib
        %compiler% %optimlvl% -c %crt0% -o %crt0obj% %arch% -ffreestanding -nostdlib
        %link% %bootobj% %crt0obj% %obj% %ldarch% %ldargs% %emulation% -o %ldout% -T %ldscript% %optionalldargs%
    ) else (
        echo Linking %src%...
        %link% %obj% %ldarch% %ldargs% %emulation% -o %ldout% -T %ldscript% %optionalldargs%
    )
    goto :ERRLVL
)
: if given src is .s file
if "%src:~-2%" == ".s" (
    echo Assembling %src% with %asm%...

    %asm% %arch% %src% -o %obj% 
    echo Linking %src%...
    %link% %obj% %ldarch% %emulation% -o %ldout% -T %ldscript% %optionalldargs%
    : del %obj%
    goto :ERRLVL
)
:INVALIDFILE
: if given src is not .c or .s file
echo Error: %src% is not a .c or .s file
goto :EOFEXIT

:ERRLVL
if %errorlevel% neq 0 (
:FAIL
    echo Error: Compilation failed.
    goto :clean
)

:RAWHEX
echo Extracting raw .text and .data bytes from %ldout% to %binoutdir%...
: extract raw .text bytes from elf file
%objcopy% -O binary --only-section=.text %ldout% %~n1.text
: extract raw .data bytes from elf file
%objcopy% -O binary --only-section=.data %ldout% %~n1.data 
if defined arg_nodata (
    echo Merging .text and.data bytes into single ^*.text file 
    if %errorlevel% neq 0 (
        echo Error: Extracting raw bytes with objcopy failed.
        goto :EOFEXIT
    ) else (
        copy /B/Y %~n1.text + %~n1.data %~n1.text
    )
) 

: if optimization defined, wipe related folder, remake it, and fill it with new files (do not ask overwrite/confirm)
if not exist %binoutdir% mkdir %binoutdir%
if defined optimlvl (
    if exist %binoutdir%\%optimlvl% RMDIR %binoutdir%\%optimlvl% /S/Q
    mkdir %binoutdir%\%optimlvl%
    set "binoutdir=%binoutdir%\%optimlvl%\"
) else (
    del %binoutdir%\* /Q
)
xcopy %~n1.text %binoutdir% /Y/Q
if not defined arg_nodata (
    xcopy %~n1.data %binoutdir% /Y/Q
)

if not defined arg_nodasm (
    echo Disassembling %ldout%...

    if exist %binoutdir%%~n1_disasm.txt del %binoutdir%%~n1_disasm.txt /Q
    %disasm% %disasmopt% %ldout% > %binoutdir%%~n1_disasm.txt
    timeout 3

    if %errorlevel% neq 0 (
        echo Error: Disassembling failed.
        goto :EOFEXIT
    ) else (
        if not defined arg_nshow (
            start %texteditor% %binoutdir%%~n1_disasm.txt
        )
    )
) 

if defined arg_outelf (
    echo Creating copy of %ldout% file...
    xcopy %ldout% %binoutdir% /Y/Q
)

echo ^[OK^] Files written to %binoutdir%


if exist *.text del %~n1.text /Q
if exist *.data del %~n1.data /Q

:clean
echo:
if not defined arg_nclean (
    echo Cleaning intermediate files...
    if exist *.o del *.o /Q
    if exist *.elf del *.elf /Q
) else (
    echo Info: --clean: intermediate files are not cleaned.
)


:EOFEXIT
endlocal
exit /b

:argunknown
    echo: Unknown argument passed: %invalidarg%
    echo Seek help

:help
    echo _____________________________________
    echo Usage: %0 srcfile ^[options^]
    echo:
    echo     srcfile : main entry .s or .c source code
    echo     optimize: [optional] optimization level for ".c" files, (-O0, -O1, -O2, -O3, -Os, -Oz, -Ofast)
    echo:
    echo    --nclean : do not clean intermediate files
    echo    --nodasm : do not disassemble the elf file
    echo    --nshow  : do not automatically show disassembled file in provided text editor
    echo    --noboot : do not include crt0.s and boot.c code (only valid for .c files)
    echo    --nodata : do not create separate "*.data" file - append .data section to eof "*.text"
    echo    --outelf : copy intermediate output "*.elf" file to out directory before removing it
    echo    --fplib  : link with "RVfplib" optimized for performance, floating-point library
    echo    --help   : show this message and exit
    goto :EOFEXIT