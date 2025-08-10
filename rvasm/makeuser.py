'''
e.g:
python .\makeuser.py .\terminal\hello.c --address 0x80000 --make-args=--nshow,--nodasm
'''
import argparse
import subprocess
import os
import re

MAKE_BAT_SCRIPT:str = "make.bat"
MAKE_BAT_FLAGS:'list[str]' = ["--nodata"]
LINKER_FILE:str = "userlink.ld"
PROGRAM_ORIGIN_REGEX:re.Pattern = re.compile(r"(__program_origin\s*=\s*)(0x[0-9A-Fa-f]+)(\s*;.*)")
OUTPUT_DIR:str = os.path.join(os.path.dirname(__file__), "..", "rvbin")
OUTPUT_MAP_FILE_EXT:str = ".map"
MAP_BYTES_PER_LINE:int = 16

def parse_args():
    parser = argparse.ArgumentParser(description="Compile and dump binary for processor terminal.")
    parser.add_argument("file", help="Source file to compile")
    parser.add_argument("--address", help="Start load address (hex, e.g. 0x20000)")
    parser.add_argument("--make-args", nargs=argparse.REMAINDER,
                        help="Extra arguments passed to make.bat")
    return parser.parse_args()

def read_program_origin() -> int:
    """Read __program_origin from linker file."""
    with open(LINKER_FILE, "r") as f:
        for line in f:
            m = PROGRAM_ORIGIN_REGEX.search(line)
            if m: 
                m = m.group(2)
                return int(m, 16 if m.startswith('0x') else 10)  
    raise RuntimeError(f"Could not find __program_origin in {LINKER_FILE}")

def overwrite_program_origin(new_addr:int) -> None:
    """Overwrite __program_origin in linker file."""
    with open(LINKER_FILE, "r") as f:
        lines = f.readlines()
    new_lines = []
    for line in lines:
        m = PROGRAM_ORIGIN_REGEX.search(line)
        if m:
            line = f"{m.group(1)}0x{new_addr:08X}{m.group(3)}\n"
        new_lines.append(line)
    with open(LINKER_FILE, "w") as f:
        f.writelines(new_lines)

def run_make(file:str, make_args:list) -> None:
    """Run make.bat with given arguments."""
    cmd = [MAKE_BAT_SCRIPT, file, f"--link+{LINKER_FILE}"]
    if MAKE_BAT_FLAGS:
        cmd.extend(MAKE_BAT_FLAGS)
    if make_args:
        cmd.extend(make_args)
    print("Running:", " ".join(cmd))
    subprocess.check_call(cmd, shell=True)

def get_bin_path(file:str) -> str:
    """Constructs output path of binary file produced by make.bat."""
    file_base = os.path.splitext(os.path.basename(file))[0]
    bin_dir = os.path.abspath(os.path.join(OUTPUT_DIR, file_base))
    bin_file = os.path.join(bin_dir, file_base + ".text")
    if not os.path.isfile(bin_file):
        raise RuntimeError(f"Binary not found: {bin_file}")
    return bin_file

def format_map(bin_file:str, start_addr:int) -> str:
    """Read binary and produce formatted mapfile string."""
    with open(bin_file, "rb") as f:
        data = f.read()

    lines = []
    for offset in range(0, len(data), MAP_BYTES_PER_LINE):
        chunk = data[offset:offset+MAP_BYTES_PER_LINE]
        hex_bytes = " ".join(f"{b:02X}" for b in chunk)
        lines.append(f"{start_addr+offset:X}: {hex_bytes}")
    return "\n".join(lines)

def main():
    args = parse_args()
    original_addr:'None|int' = None
    addr_to_use:int

    if not args.address:
        addr_to_use = read_program_origin()
    else:
        addr_to_use = int(args.address, 16)
        temp_original = read_program_origin()
        if temp_original != addr_to_use:
            original_addr = temp_original
            overwrite_program_origin(addr_to_use)

    print(f"[INFO] Program origin: 0x{addr_to_use:08X}")
    try:
        print(f"[INFO] Running {MAKE_BAT_SCRIPT} script...")
        run_make(args.file, args.make_args)
        print("[INFO] Sucessfull!")
    finally:
        if original_addr is not None:
            print(f"[INFO] Restoring the original address in linker script: 0x{original_addr:08X}")
            overwrite_program_origin(original_addr)

    print(f"[INFO] Localizing binary of {args.file}...")
    bin_file = get_bin_path(args.file)
    print(f"[INFO] Formatting {OUTPUT_MAP_FILE_EXT} file...")
    out_text = format_map(bin_file, addr_to_use)

    out_text_file = os.path.splitext(bin_file)[0] + OUTPUT_MAP_FILE_EXT
    with open(out_text_file, "w") as f:
        f.write(out_text)

    print(f"[INFO] Finished: Map file saved to {out_text_file}")

if __name__ == '__main__':
    try:
        main()
    except Exception as e:
        print(f'[ERROR] Programm will close due to exception: {e}')