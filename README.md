### Simulator of pipelined, superscalar processor core based on RISC-V instruction set

Master's thesis project supporting reasearch of performance gains resulting from the superscalar architecture use with an integer RISC-V ISA.

Application is a reasearch/educational tool providing a simulation environment capable of running a 32-bit RISC-V binaries on a classic five-step 
scalar pipeline or a configurable dynamic superscalar pipeline. The simulator supprots step-by-step mode as well as full-speed execution (with breakpoints), 
which allows the user to observe program execution and collect information regarding dataflow in the processing unit.

---

Application is designed exclusively for machines running *Windows* OS. 
The project source is divided into two main parts: 
- [simulator logic](superscalar_arch_sim),
- [interface and visualization](superscalar_arch_sim_gui), using the *Windows Forms* framework, requiring a runtime environment for .NET Framework 4.2.7. 

Created user interface is presented in the [screenshots](./screenshots) folder.

#### Environment overview

Compilation and linking are done independently and require the use of an existing toolchain (e.g. GCC `riscv64-unknown-elf...` suite). 
User can utilize provided configuration by installing RISC-V GCC toolchain and setting appropriate paths in Windows Batch script [make.bat](./rvasm/make.bat) located in [rvasm](./rvasm) folder.

By default, script automatically invokes the compilation of a single-file `.c` or `.s` source and links it with 
[boot](./rvasm/system/boot.c) and [crt0](./rvasm/system/crt0.s) environment initialization code. 
Predefined linker script [simlink.ld](./rvasm/simlink.ld) is used. Resulting binaries are placed in the [rvbin](./rvbin) directory.

For more options, see `make.bat --help` or explore the source.

##### Instruction set

The simulator allows running programs from binary files, compiled under the 32-bit RISC-V integer ISA (**I** and **M** extensions).
It does not support the instructions of **F** and **D** extensions; instead, when running programs that use floating-point calculations, 
they should be linked with library providing software emulation of fp operations compatible with the *IEEE754* standard 
for single and double-precision numbers, e.g. [RVfplib](https://github.com/pulp-platform/RVfplib).

Simulation does not support the specification-defined *Control Status Registers* and system instructions `ECALL`/`FENCE`. 
The `EBREAK` instruction is used to automatically pause the execution when executed.

##### Memory

The compiled program is placed in the simulated processor's read-only memory. If necessary, additional data can also be uploaded to the simulated random access memory.
The implementation allows access to the **full 32-bit address space** through the use of a dictionary data structure, storing words indexed by an address.
This approach makes it possible to simulate access to arbitrary address ranges without pre-allocation of large memory resources.

Reads and writes in the simulator support only *aligned* accesses when using load/store instructions for a word, half word, and byte. 
In the case of unaligned access, an error will be generated in the application, which does not allow the to simulate the occurrence of such a situation in the processor. 
The user must ensure aligned memory access in running programs, and software or hardware handling of such errors is not included in the adopted model.

Program execution begins at address 0. By default, the first 65536 bytes of the address space are configured as *read-only* memory. The rest is configured
as free-access operational memory. An attempt to write to program memory or access to the address space outside the configured address space, results in an
generation of an error in the application. This allows early verification of program execution errors, if such a situation should not occur.

##### Superscalar architecture 

The simulated superscalar pipeline consists of six stages and three main types of execution units; for branch, integer and memory access instructions. 
It utilizes an extended [Tomasulo alghoritm](https://en.wikipedia.org/wiki/Tomasulo%27s_algorithm) for *speculative, out-of-order execution*. 
Fetched and decoded instructions are placed in an instruction queue, from which they are then issued to the individual *reservation stations* and a *reorder buffer*. 
It is not possible to disable the speculative mode and the jump prediction functionality.

The following superscalar pipeline configuration parameters are available:

|  |  |
|---|---|
| `NFW` | Maximum number of instructions processed in a single cycle at the *fetch* and *decode* stage. |
| `NIW` | The maximum number of instructions that can be issued to execution units in a single cycle. |
| `LIQ` | The size of the instruction queue between the *decode* and *dispatch* stages. |
| `LRS` | Number of positions in the reservation station corresponding to a specific execution unit. |
| `LROB` | Maximum number of items in the reorder buffer. |
| `NMEM` | Number of execution units for memory access instructions. |
| `NINT` | Number of execution units for integer ALU operations. |

The flow of instructions with respect to the various stages of the pipeline and existing configurations looks as follows:

- **Fetch** - a maximum of `NFW` sequential instructions are fetched from memory.
If a jump instruction with a known destination address is fetched, the sequence of the
block of instructions is interrupted, and a new block will be fetched on the next cycle.

- **Decode** - a maximum of `NFW` instructions are decoded. If there is space for a
full block, it is written to the instruction queue with the maximum `LIQ` size.
Otherwise, fetching and decoding is stopped.

- **Dispatch** - at most `NFW` instructions are placed in the corresponding reservation stations (size `LRS`) and reorder buffer (size `LROB`). For
each of the dispatched instructions, ready arguments are read or assigned a
producer tag. Upon encountering the first instruction for which the target buffer is full, the process is halted and the remaining instructions wait in the queue
for the release of hardware resources.

- **Execute** - a maximum of `NIW` ready instructions from reservation stations are issued
to the corresponding execution units in which, depending on the type of unit:
	- the target address and/or the condition of at most one jump instruction are resolved,
	- a maximum of `NMEM` memory read operations are performed (Load) or calculations of the target address for write instructions (Store).
	- maximum `NINT` ALU operations are performed.

- **Complete** - the results of all executed instructions (no more than the sum of `NMEM` and `NINT`) update the arguments of pending operations 
in the reservation stations. In case of an incorrect address prediction or jump condition, all “newer” instructions are discarded, and the corresponding address 
is sent to the fetch unit.

- **Retire** - with the fetch order preserved, all instructions that have completed the previous stage are removed from the reorder buffer. The values generated by 
the corresponding operations are written to registers and the argument of the Store
is sent to memory.

There is an option to set additional execution flags, allowing speculative access to memory via *Load* instructions or using an additional data bus, 
through which the value from the *Store* instruction will be sent to the dependent *Load*, bypassing memory, provided that these instructions 
are not executed speculatively (*Store-Load Bypass*).

---
