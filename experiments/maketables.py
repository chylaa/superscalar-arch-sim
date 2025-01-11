import os, sys, json, re

CONFIGS:dict = {
    1: {'N_FW': 1, 'N_IW': 1, 'L_IQ': 8, 'L_ROB': 10, 'N_MEM': 1, 'N_INT': 1},
    2: {'N_FW': 1, 'N_IW': 2, 'L_IQ': 8, 'L_ROB': 20, 'N_MEM': 2, 'N_INT': 2},
    3: {'N_FW': 1, 'N_IW': 4, 'L_IQ': 8, 'L_ROB': 36, 'N_MEM': 4, 'N_INT': 4},
    4: {'N_FW': 1, 'N_IW': 8, 'L_IQ': 8, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8},
    5: {'N_FW': 2, 'N_IW': 1, 'L_IQ': 16, 'L_ROB': 10, 'N_MEM': 1, 'N_INT': 1},
    6: {'N_FW': 2, 'N_IW': 2, 'L_IQ': 16, 'L_ROB': 20, 'N_MEM': 2, 'N_INT': 2},
    7: {'N_FW': 2, 'N_IW': 4, 'L_IQ': 16, 'L_ROB': 36, 'N_MEM': 4, 'N_INT': 4},
    8: {'N_FW': 2, 'N_IW': 8, 'L_IQ': 16, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8},
    9: {'N_FW': 4, 'N_IW': 1, 'L_IQ': 24, 'L_ROB': 10, 'N_MEM': 1, 'N_INT': 1},
    10: {'N_FW': 4, 'N_IW': 2, 'L_IQ': 24, 'L_ROB': 20, 'N_MEM': 2, 'N_INT': 2},
    11: {'N_FW': 4, 'N_IW': 4, 'L_IQ': 24, 'L_ROB': 36, 'N_MEM': 4, 'N_INT': 4},
    12: {'N_FW': 4, 'N_IW': 8, 'L_IQ': 24, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8},
    13: {'N_FW': 6, 'N_IW': 1, 'L_IQ': 32, 'L_ROB': 10, 'N_MEM': 1, 'N_INT': 1},
    14: {'N_FW': 6, 'N_IW': 2, 'L_IQ': 32, 'L_ROB': 20, 'N_MEM': 2, 'N_INT': 2},
    15: {'N_FW': 6, 'N_IW': 4, 'L_IQ': 32, 'L_ROB': 36, 'N_MEM': 4, 'N_INT': 4},
    16: {'N_FW': 6, 'N_IW': 8, 'L_IQ': 32, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8},
    17: {'N_FW': 8, 'N_IW': 1, 'L_IQ': 64, 'L_ROB': 10, 'N_MEM': 1, 'N_INT': 1},
    18: {'N_FW': 8, 'N_IW': 2, 'L_IQ': 64, 'L_ROB': 20, 'N_MEM': 2, 'N_INT': 2},
    19: {'N_FW': 8, 'N_IW': 4, 'L_IQ': 64, 'L_ROB': 36, 'N_MEM': 4, 'N_INT': 4},
    20: {'N_FW': 8, 'N_IW': 8, 'L_IQ': 64, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8},   
    21: {'N_FW': 6, 'N_IW': 8, 'L_IQ': 64, 'L_ROB': 68, 'N_MEM': 8, 'N_INT': 8}, 
}
DISCARD_CONFIGS:'list[int]' = []

# Example file name: D-ooo-1fetch-2issue-8queue-20rob-1brch4-2mem4-2int4-BranchHistoryTable.txt
def get_config_and_id(config_filename) -> 'tuple[int, dict]':  
    global CONFIGS
    config = { 'N_FW': 0, 'N_IW': 0, 'L_IQ': 0, 'L_ROB': 0, 'N_MEM': 0, 'N_INT': 0 }
    config['N_FW'] = int(re.search(r'(\d+)fetch', config_filename).group(1))
    config['N_IW'] = int(re.search(r'(\d+)issue', config_filename).group(1))
    config['L_IQ'] = int(re.search(r'(\d+)queue', config_filename).group(1))
    config['L_ROB'] = int(re.search(r'(\d+)rob', config_filename).group(1))
    config['N_MEM'] = int(re.search(r'(\d+)mem', config_filename).group(1))
    config['N_INT'] = int(re.search(r'(\d+)int', config_filename).group(1))

    for key, value in CONFIGS.items():
        if all(value[k] == config[k] for k in value.keys()):
            return key, config
    return -1, None

def get_experiment_data(base_folder):
    data = []
    
    for subdir, dirs, files in os.walk(base_folder):
        for file in files:
            if str(file).startswith("S-"):
                continue
            if str(file).endswith("BranchHistoryTable.txt"):
                # Extract configuration ID from the filename
                config_id, config = get_config_and_id(file) 
                fetch_width, issue_width = config['N_FW'], config['N_IW']
                
                # Read the associated JSON and TXT files
                json_file = os.path.join(subdir, 'D-measures-aggregate.json')
                txt_file = os.path.join(subdir, 'D-simcounters.txt')
                hist_file = os.path.join(subdir, 'D-measures-histograms.txt')
                
                with open(json_file, 'r') as f:
                    aggregate_data = json.load(f)
                with open(txt_file, 'r') as f:
                    simcounters_data = f.readlines()
                with open(hist_file, 'r') as f:
                    hist_data = f.readlines()
                
                # Parse data for metrics
                ipc = float([line for line in simcounters_data if 'IPC' in line][0].split('=')[1].strip())
                clock_cycles = int([line for line in simcounters_data if 'Clock Cycles' in line][0].split('=')[1].strip())
                
                committed_instructions = int([line for line in simcounters_data if 'Commited' in line][0].split('=')[1].strip())
                pd:float = ((1 - (float)(ipc / issue_width)) * 100)
                
                # Parse "::[UnitFull]," section for B_full metrics
                unit_full = { } 
                for i, line in enumerate(hist_data):
                    if i > 0 and i < 6:
                        line = line.strip()[1:-1]
                        unit_full[line.split(',')[0].strip()] = int(line.split(',')[1].strip())
                    if i >= 6:
                        break
                
                # Extract other values
                t_avg = (aggregate_data['ProcessingCycles']['Average'] + 1)
                
                r_avg_if = aggregate_data['Fetch_Throughput']['Average']
                r_avg_id = aggregate_data['Dispatch_Throughput']['Average']
                r_avg_ex = aggregate_data['Execute_Througput']['Average']
                r_avg_wb = aggregate_data['Retire_Througput']['Average']
                
                b_avg_rob       = 100 * (aggregate_data['ROB_Size']['Average'] / config['L_ROB'])
                b_avg_iq        = 100 * (aggregate_data['IRQueue_Size']['Average'] / config['L_IQ'])
                b_avg_branch    = 100 * (aggregate_data['BranchUnit_Size']['Average'] / (4 * 1))
                b_avg_int       = 100 * (aggregate_data['IntUnit_Size']['Average'] / (4 *config['N_INT']))
                b_avg_mem       = 100 * (aggregate_data['MemUnit_Size']['Average'] / (4 * config['N_MEM']))
                
                b_full_rob      = 100 * (unit_full['ROB'] / clock_cycles)
                b_full_iq       = 100 * (unit_full['IRQueue'] / clock_cycles) 
                b_full_branch   = 100 * (unit_full['BranchUnit'] / clock_cycles) 
                b_full_int      = 100 * (unit_full['IntUnit'] / clock_cycles)
                b_full_mem      = 100 * (unit_full['MemUnit'] / clock_cycles)
                
                # Collect values into a dictionary
                data.append({
                    'N_FW': fetch_width,
                    'N_IW': issue_width,
                    'ID': config_id,
                    'IPC': ipc,
                    'PD': pd,
                    'T_avg': t_avg,
                    'R_avg_IF': r_avg_if,
                    'R_avg_ID': r_avg_id,
                    'R_avg_EX': r_avg_ex,
                    'R_avg_WB': r_avg_wb,
                    'B_avg_ROB': b_avg_rob,
                    'B_avg_IQ': b_avg_iq,
                    'B_avg_Branch': b_avg_branch,
                    'B_avg_Int': b_avg_int,
                    'B_avg_Mem': b_avg_mem,
                    'B_full_ROB': b_full_rob,
                    'B_full_IQ': b_full_iq,
                    'B_full_Branch': b_full_branch,
                    'B_full_Int': b_full_int,
                    'B_full_Mem': b_full_mem
                })
    return data

def calculate_scalars(base_folder, superscalar_data):
    scalar_ipc = None
    
    # Find scalar results
    for subdir, dirs, files in os.walk(base_folder):
        if 'S-' in subdir:
            for file in files:
                if file.endswith("S-simcounters.txt"):
                    txt_file = os.path.join(subdir, file)
                    with open(txt_file, 'r') as f:
                        simcounters_data = f.readlines()
                    
                    # Extract IPC for scalar
                    scalar_ipc = float([line for line in simcounters_data if 'IPC' in line][0].split('=')[1].strip())
                    break
    
    # Calculate S_R and S based on scalar IPC
    for entry in superscalar_data:
        entry['S_R'] = entry['IPC'] / scalar_ipc
        entry['S'] = entry['IPC'] / 1.0  # Ideal scalar IPC = 1.0
    
    return superscalar_data

def data_pass_suitability_indicator(entry, alldata)->bool:
    return abs(entry['N_FW'] - entry['N_IW']) < 5 

def generate_latex_table(data, benchmark:str):
     # Generating the main performance table
    latex_table = ""
    latex_table += "\n\\begin{table}[]"
    latex_table += "\n\\centering"
    latex_table += "\n\\caption{" + " Zebrane miary wydajności potoku dla konfiguracji programu \\texttt{" + benchmark.replace('_', '\\_') + "}. }"
    latex_table += "\n\\label{tab:results_" + benchmark + "}"
    latex_table += "\n\\begin{tabular}{|>{\\columncolor[HTML]{EFEFEF}}c|c|>{\\columncolor[HTML]{EFEFEF}}c|c|>{\\columncolor[HTML]{EFEFEF}}c|cccc|}"
    latex_table += "\n\\hline"
    
    # First header row: label for R_avg columns
    latex_table += "\\cellcolor[HTML]{EFEFEF} & & \\cellcolor[HTML]{EFEFEF} & &  \\cellcolor[HTML]{EFEFEF} & \\multicolumn{3}{c|}{$\\mathbf{R_{avg}}$} \\\\ \\cline{6-8}\n"
    
    # Second header row: actual column names
    latex_table += "\\multirow{-2}{*}{\\cellcolor[HTML]{EFEFEF}\\textbf{ID}} & \\multirow{-2}{*}{$\mathbf{IPC/S}$} & "
    latex_table += "\\multirow{-2}{*}{\\cellcolor[HTML]{EFEFEF}$\\mathbf{PD}$} & \\multirow{-2}{*}{$\\mathbf{S_R}$} & "
    latex_table += "\\multirow{-2}{*}{\\cellcolor[HTML]{EFEFEF}$\\mathbf{T_{avg}}$}  & "
    latex_table += "\\multicolumn{1}{c|}{IF} & \\multicolumn{1}{c|}{\\cellcolor[HTML]{EFEFEF}ID} & "
    latex_table += "\\multicolumn{1}{c|}{EX} \\\\ \\hline\n"
    
    # Adding data rows
    for i, entry in enumerate(data):
        if i in DISCARD_CONFIGS or not data_pass_suitability_indicator(entry, data):
            continue
        latex_table += f"\\cellcolor[HTML]{{EFEFEF}}{entry['ID']} & {entry['IPC']:.3f} & "
        latex_table += f"\\cellcolor[HTML]{{EFEFEF}}{entry['PD']:.3f} & {entry['S_R']:.3f} & "
        latex_table += f"\\cellcolor[HTML]{{EFEFEF}}{entry['T_avg']:.3f} & "
        latex_table += f"{entry['R_avg_IF']:.3f} & "
        latex_table += f"\\cellcolor[HTML]{{EFEFEF}}{entry['R_avg_ID']:.3f} & "
        latex_table += f"{entry['R_avg_EX']:.3f} \\\\ \\hline\n"
    
    latex_table += "\\end{tabular}\n\\end{table}\n\n"
    
    latex_table += "\n\\begin{table}[]"
    latex_table += "\n\\centering"
    latex_table += "\n\\caption{" + " Wykorzystanie buforów dla dla konfiguracji programu \\texttt{" + benchmark.replace('_', '\\_') + "}. }"
    latex_table += "\n\\label{tab:buffers_" + benchmark + "}"
    latex_table += "\n\\begin{tabular}{|c|cc|cc|cc|cc|cc|}"
    latex_table += "\n\\hline"
    
    # Header for buffer table
    latex_table += "& "
    latex_table += "\\multicolumn{2}{c|}{\\textbf{ROB}} & "
    latex_table += "\\multicolumn{2}{c|}{\\textbf{IQ}} & "
    latex_table += "\\multicolumn{2}{c|}{$\\mathbf{RS_{Branch}}$} & "
    latex_table += "\\multicolumn{2}{c|}{$\\mathbf{RS_{Int}}$} & "
    latex_table += "\\multicolumn{2}{c|}{$\\mathbf{RS_{Mem}}$} \\\\ \\cline{2-11}\n"
    
    # Sub-header for buffer table
    latex_table += "\\multirow{-2}{*}{\cellcolor[HTML]{EFEFEF}\\textbf{ID}} & "
    latex_table += "$\\mathbf{B_{avg}}$ & \\cellcolor[HTML]{EFEFEF}$\\mathbf{B_{full}}$ & "
    latex_table += "$\\mathbf{B_{avg}}$ & \\cellcolor[HTML]{EFEFEF}$\\mathbf{B_{full}}$ & "
    latex_table += "$\\mathbf{B_{avg}}$ & \\cellcolor[HTML]{EFEFEF}$\\mathbf{B_{full}}$ & "
    latex_table += "$\\mathbf{B_{avg}}$ & \\cellcolor[HTML]{EFEFEF}$\\mathbf{B_{full}}$ & "
    latex_table += "$\\mathbf{B_{avg}}$ & \\cellcolor[HTML]{EFEFEF}$\\mathbf{B_{full}}$ \\\\ \\hline\n"

    # Adding data rows for the buffer table
    for i, entry in enumerate(data):
        if i in DISCARD_CONFIGS or not data_pass_suitability_indicator(entry, data):
            continue
        # Multirow for the ID column (spans 2 rows)
        latex_table += f"{entry['ID']} & "
        latex_table += f"{entry['B_avg_ROB']:.3f} & \\cellcolor[HTML]{{EFEFEF}}{entry['B_full_ROB']:.3f} & "
        latex_table += f"{entry['B_avg_IQ']:.3f} & \\cellcolor[HTML]{{EFEFEF}}{entry['B_full_IQ']:.3f} & "
        latex_table += f"{entry['B_avg_Branch']:.3f} & \\cellcolor[HTML]{{EFEFEF}}{entry['B_full_Branch']:.3f} & "
        latex_table += f"{entry['B_avg_Int']:.3f} & \\cellcolor[HTML]{{EFEFEF}}{entry['B_full_Int']:.3f} & "
        latex_table += f"{entry['B_avg_Mem']:.3f} & \\cellcolor[HTML]{{EFEFEF}}{entry['B_full_Mem']:.3f} \\\\ \\hline\n"

    latex_table += "\\end{tabular}\n\\end{table}\n"

    return latex_table

    

def main():
    base_folders:list[str] = []
    if len(sys.argv) > 1:
        base_folders = sys.argv[1:]
    else:
        print('Need to provide at least one base folder')
        sys.exit(1)
    
    for base_folder in base_folders:
        superscalar_data = get_experiment_data(base_folder)
        full_data = calculate_scalars(base_folder, superscalar_data)
        
        benchmark = os.path.basename(base_folder if base_folder[-1] != '\\' else base_folder[:-1])
        latex_table = generate_latex_table(full_data, benchmark)
        
        with open(base_folder + "\\results.tex", "wb") as f:
            f.write(latex_table.encode('utf-8'))
            
        print(f"Results for {base_folder} saved to results.tex")

if __name__ == "__main__":
    main()