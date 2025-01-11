import os
import re
import sys

PREDICTOR_SUFIX:str = "BranchHistoryTable"
counters_filters:dict = {
    'D-' : [
        "Data Dependencies =",
        "Address misspredictions =",
        "Feedback MEM LMD->AddrIn =",
        "Feedback ALU Out->In =" ,
        "Forward MEM LMD to ALU =", 
        "Load Interlocks ="
    ],
    'S-' : [
        "Data Dependencies =",
        "Address misspredictions =",
        "Store Load Bypasses ="
    ]
}

settings_filters:dict = {
    'D-' : [
        "SettingsChanged=","ROM", "RAM", "ThrowOn", "ProgramStart",
        "Static_"
    ],
    'S-' : [
        "SettingsChanged =", "ROM", "RAM", "ThrowOn", "ProgramStart",
        "Dynamic_",
        "CoreMode",
        "InstructionQueueCapacity = ",
        "NumberOfMemoryFunctionalUnits = ",
        "NumberOfIntegerFunctionalUnits = ",
        "NumberOfReorderBufferEntries = ",
        "NumberOfBranchReservationStations = ",
        "NumberOfIntegerReservationStations = ",
        "NumberOfMemoryReservationStationBuffers = ",
        "NumberOfReservationStationsPerMemoryUnit = ",
        "NumberOfReservationStationsPerIntegerUnit = ",
        "TotalNumberOfExecutionUnits = ",
        "TotalNumberOfReservationStations = "
    ]
}

def trim_histogram(file_content):
    """Trims the trailing zero-filled entries from the histograms in the 'measures-histograms.txt' file."""
    trimmed_content = []
    current_histogram = []
    trimming_ended = False

    for line in reversed(file_content):
        # Detect the start of a new histogram block (this resets trimming)
        if '::' in line:
            # Insert the previous histogram block (if any) after trimming
            if current_histogram:
                trimmed_content = current_histogram + trimmed_content
                current_histogram = []
            trimming_ended = False
            trimmed_content.insert(0, line)
            continue

        # If it's part of the histogram data (in brackets and with commas)
        strpline = line.strip()
        if len(strpline) > 0 and ',' in strpline and strpline.startswith('[') and strpline.endswith(']'):
            numbers = strpline[1:-1].split(',')
            num0 = numbers[0].strip()
            num1 = numbers[1].strip()

            # Check if it has two numbers and the second number is zero
            if len(numbers) == 2 and num1.isdigit() and int(num1) == 0 and not trimming_ended:
                # Don't append zero-filled lines until we hit a non-zero line
                continue
            else:
                trimming_ended = True  # Found a non-zero, stop trimming

        # Add the line to the current histogram block
        current_histogram.insert(0, line)

    # Add the last histogram block after trimming
    if current_histogram:
        trimmed_content = current_histogram[::-1] + trimmed_content

    return trimmed_content
    
def filter_file(file_prefix:str, file_content:list, filters:dict):
    filtered_content = []
    for line in (file_content):
        if all(_filter not in line for _filter in filters[file_prefix]):
            filtered_content.append(line)
            
    return filtered_content
    
def get_file_prefix(folder_name):
    """Determine the file prefix (either 'D-' or 'S-') based on the folder name."""
    if folder_name.startswith('S-'):
        return 'S-'
    return 'D-'

def find_settings_file(files, file_prefix):
    """Find the settings file (*{PREDICTOR_SUFIX}.txt) with dynamic names."""
    for file_name in files:
        if re.match(f"{file_prefix}.*{PREDICTOR_SUFIX}.txt", file_name):
            return file_name
    return None

def merge_files(root_dir, output_file):
    """Merge all files from subdirectories into one big text file in the root directory."""
    # Open the output file in write mode
    with open(output_file, 'w') as outfile:
        # Walk through the root directory and all its subdirectories
        for subdir, _, files in os.walk(root_dir):
            folder_name = os.path.basename(subdir)

            # Only process folders that match the report name pattern (e.g., "D-report-..." or "S-report-...")
            if folder_name.startswith(('D-report', 'S-report')):
                file_prefix = get_file_prefix(folder_name)

                # Find the settings file dynamically based on its varying name
                settings_file = find_settings_file(files, file_prefix)

                # Define other known files to process, adjusting the prefix dynamically
                other_files = [
                    f"{file_prefix}measures-aggregate.json",
                    f"{file_prefix}measures-histograms.txt",
                    f"{file_prefix}simcounters.txt"
                ]

                # Process the settings file first
                if settings_file and os.path.exists(os.path.join(subdir, settings_file)):
                    file_path = os.path.join(subdir, settings_file)
                    outfile.write(f"// ============ {settings_file} in {os.path.basename(subdir)} ============\n")
                    with open(file_path, 'r') as infile:
                        file_content = filter_file(file_prefix, infile.readlines(), settings_filters) 
                        outfile.writelines(file_content)

                # Process the other known files in the correct order
                for file_name in other_files:
                    file_path = os.path.join(subdir, file_name)
                    if os.path.exists(file_path):
                        outfile.write(f"// ============ {file_name} in {os.path.basename(subdir)} ============\n")
                        with open(file_path, 'r') as infile:
                            file_content = infile.readlines()
                            
                            # If the file is '*simcounters.txt', filter unused parameters
                            if "simcounters.txt" in file_name:
                                file_content = filter_file(file_prefix, file_content, counters_filters) 
                                
                            # If the file is '*measures-histograms.txt', trim trailing zeros
                            if "measures-histograms.txt" in file_name:
                                file_content = trim_histogram(file_content)

                            # Write the file content to the output
                            outfile.writelines(file_content)

        print(f"Merged files successfully into {output_file}")

def main():
    print('')
    root_directory_list = []
    if len(sys.argv) > 1:
        root_directory_list.extend(sys.argv[1:])
    else:       
        root_directory:str = input("Enter the root directory path where the report folders are located or multiple paths separated with space: ")
        if (not root_directory.startswith('"')) and ' ' in root_directory:
            root_directory_list.extend(root_directory.split(' ')) 
        else:
            root_directory_list.append(root_directory)
    count = 0
    for rootdir in root_directory_list:
        output_filename = os.path.join(rootdir, '.merged_reports.txt')
        merge_files(rootdir, output_filename)
        count += 1
    print(f'\nSuccessfully merged {count} files')
    
if __name__ == "__main__":
    main()
