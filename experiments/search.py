import os
import re
import json
import argparse
import traceback

parser = argparse.ArgumentParser(description="Process files based on a pattern and a command.")
parser.add_argument("directory", type=str, help="The directory to search within.")
parser.add_argument("file_pattern", type=str, help="Regex pattern for matching file names.")
parser.add_argument("in_file_param", type=str, help="The parameter to search for within files.")
parser.add_argument("command", type=str, help="The command to execute (param-list, json-list, param-max, param-min)")
parser.add_argument("--fulldir", "-f", action='store_true', help="If specified, paths will be printed including base 'directory' component")

args = parser.parse_args()
arg_directory = args.directory
arg_file_pattern = args.file_pattern
arg_in_file_param = args.in_file_param
arg_command = args.command
arg_fulldir = args.fulldir

def find_files(directory, pattern):
    """Recursively find all files in a directory matching a specific pattern."""
    matches = []
    for root, _, files in os.walk(directory):
        for filename in files:
            if re.match(pattern, filename):
                matches.append(os.path.join(root, filename))
    return matches

def process_file(file_path, in_file_param, command):
    """Process a file based on the command specified."""
    if command == "param-list":
        with open(file_path, 'r') as f:
            for line in f:
                if in_file_param in line:
                    print_path = file_path if arg_fulldir else file_path.replace(arg_directory, "")
                    print(f"{print_path}: {line.strip()}")
                    return

    elif command == "json-list":
        with open(file_path, 'r') as f:
            try: data = json.load(f)
            except Exception as ex: print(f"Error loading {file_path} as JSON: {ex}")
            if in_file_param in data:
                print_path = file_path if arg_fulldir else file_path.replace(arg_directory, "")
                print(f"{print_path}: {data[in_file_param]}")

    elif "param-max" in command or "param-min" in command:
        # Extract floating-point value after '=' for param-max or param-min
        regex = re.compile(rf"{in_file_param}.+\s*=\s*([-+]?[0-9]*\.?[0-9]+)")
        with open(file_path, 'r') as f:
            for line in f:
                match = regex.search(line)
                if match:
                    value = float(match.group(1))
                    return (file_path, value)
    return None

def parse_command(command):
    """Parse the command to determine if it's param-list, json-list, param-max, or param-min, and extract the count if specified."""
    match = re.match(r"(param-(?:list|max|min)|json-list)(?:-(\d+))?", command)
    if not match:
        return None, None
    base_command = match.group(1)  # "param-list", "json-list", "param-max", or "param-min"    
    # Only parse count as an integer if it's present and numeric
    count = int(match.group(2)) if match.group(2) and match.group(2).isdigit() else 1
    return base_command, count


def main():
    print('\nBase dir:', arg_directory)
    
    # Parse command to determine if it’s param-max or param-min, and the count
    base_command, count = parse_command(arg_command)
    if base_command is None:
        print("Invalid command. Use param-list, json-list, param-max, param-min, or param-max/min with optional count.")
        return

    # Find all files matching the pattern
    files = find_files(arg_directory, arg_file_pattern)
    if not files:
        print(f'No files found matching pattern {arg_file_pattern} in {arg_directory}')
        return
    print(f'Found {len(files)} matching files')

    # Process files based on the command
    if base_command in ["param-list", "json-list"]:
        for file_path in files:
            process_file(file_path, arg_in_file_param, base_command)

    elif base_command in ["param-max", "param-min"]:
        results = []
        for file_path in files:
            result = process_file(file_path, arg_in_file_param, base_command)
            if result:
                results.append(result)

        if results:
            # Sort results by value and get the top N
            results_sorted = sorted(results, key=lambda x: x[1], reverse=(base_command == "param-max"))
            top_results = results_sorted[:count]

            print(f'{base_command.upper()} results:')
            for file_path, value in top_results:
                print_path = file_path if arg_fulldir else file_path.replace(arg_directory, "")
                print(f'{print_path} with value {value}')
        else:
            print(f'No values found for parameter "{arg_in_file_param}" in files.')

if __name__ == "__main__":
    try:
        main()
    except Exception as ex:
        print(traceback.format_exc())
        input(f'Program will close due to Exception {ex}')
