#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/stdio.h"
#include "../simlib/hex.h"

#define RX_BUFFER_SIZE 255
#define TERMINAL_MAX_CHAR 0x7F

typedef enum { 
    CC_BACKSPACE = 0x08,
    CC_RETURN = '\r',
    CC_ESCAPE = '\e',
    CC_LINEFEED = '\n',
    CC_MAX = 0x1F
} ControlChars;

static const char* promptstr = "> ";
static char cbuffer[RX_BUFFER_SIZE] = {0};
static char* bufferptr = cbuffer;

int terminal_get_echo_char() {
    int received = getc();
    int isascii = received <= TERMINAL_MAX_CHAR;
    if (isascii) {
        return putc(received);
    }
    return IO_ERROR;
}

void print_address(uint32_t addr) {
    static char buffer[8+1];
    puts(u32tohex(addr, buffer));
} 

void exec_read(uint8_t* start, const uint8_t* end) {
    do {
        putc(' ');
        putc(HEXCHARS[(*start) >> 4]);
        putc(HEXCHARS[(*start) & 0xF]);
    } while(start++ < end);
}

int is_address_char(char c) { 
    return isxdigit(c) ||  c == 'x'|| c == 'X'; 
}

// addr -> val 
// addr.addr -> val val val...
// .addr -> val val val...
void exec_command(char* buffer, int size) {
    /// TODO: Fix or add safeguard on invalid first input (without ever setting 'startaddr' to something)
    static uint32_t startaddr = 0;
    const char* p = buffer;
    
    while (is_address_char(*p++));
    // here we could safely get the non-addr symbol with "char x = *(p-1);" 
    const char* endstr = p;  
    hextou32(buffer, (p - buffer) - 1, &startaddr);
    
    while (is_address_char(*p++));
    uint32_t endaddr = 0;
    hextou32(endstr, (p - endstr) - 1, &endaddr);
    
    print_address(startaddr); putc(':');
    exec_read((uint8_t*)startaddr, (uint8_t*)endaddr);

    putc(CC_LINEFEED);
    puts(promptstr);
}

int main(void) {
    io_init();
    puts(promptstr);
    
    char* buffer = cbuffer;
    int buffer_size = 0;
    int buffer_max_size = RX_BUFFER_SIZE;
    
    for(;;) {
        int c = terminal_get_echo_char();
        if (c == IO_ERROR) {
            continue;
        }
        else if (c == CC_BACKSPACE) {
            if (buffer_size > 0) 
                --buffer_size;
            continue;
        }
        else if (c == CC_LINEFEED) {
            buffer[buffer_size] = '\0';
            exec_command(buffer, buffer_size);
            buffer[buffer_size = 0] = '\0'; // "free" buffer
            continue;
        }
        else if (c == CC_ESCAPE) {
            buffer[buffer_size = 0] = '\0';
            putc(CC_LINEFEED); puts(promptstr);
            continue;
        }
        // for now ignore rest of control characters
        else if (c <= CC_MAX) { 
            continue;
        }

        if (buffer_size < buffer_max_size) {
            buffer[buffer_size++] = c;
        } else {
            puts("\nOVF!\n");
            buffer[buffer_size = 0] = '\0';
            puts(promptstr);
        }
    }
}