#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/stdio.h"
#include "../simlib/hex.h"

#define RX_BUFFER_SIZE 255
#define TERMINAL_MAX_CHAR 0x7F

typedef enum { 
    CC_BACKSPACE = '\b',
    CC_RETURN = '\r',
    CC_ESCAPE = '\e',
    CC_LINEFEED = '\n',
    CC_MAX = 0x1F
} ControlChars;

static const char* promptstr = "> ";
static char cbuffer[RX_BUFFER_SIZE] = {0};
static char* bufferptr = cbuffer;

static uint32_t received_chars = 0;

int terminal_get_echo_char() {
    int received = getc();
    ++received_chars;
    return putc(received);
}

void print_uint32(uint32_t value) {
    static char buffer[8+1];
    puts(u32tohex(value, buffer));
} 

void exec_read(uint8_t* start, const uint8_t* end) {
    do {
        putc(' ');
        putc(HEXCHARS[(*start) >> 4]);
        putc(HEXCHARS[(*start) & 0xF]);
    } while(start++ < end);
}

uint32_t exec_write(uint8_t* dest, const char* values) {
    while (*values) {
        if (!isxdigit(*values)) {
            ++values;
            continue;
        }
        int8_t h = hexctonum(*values++);
        int8_t l = hexctonum(*values++);
        if (h < 0 && l < 0) {
            break;
        } 
        if (h >= 0 && l < 0) {
            *dest = h;
            continue;
        }
        *dest++ = ((h << 4) | l);
    }
    return (uint32_t)(dest - 1);
}

void exec_run(uint32_t startaddr) {
    int32_t (*userprogram)(void) = (int32_t (*)())startaddr;
    int32_t retcode = userprogram();
    puts("\n[EXIT: ");
    print_uint32(retcode);
    putc(']');
}

int is_address_char(char c) { 
    return isxdigit(c) || c == 'x' || c == 'X'; 
}

// ==========< READ >===========
// addr -> val 
// addr.addr -> val val val...
// .addr -> val val val...
// =========< WRITE >===========
// addr:val val val...
// :val val val...
// ==========< RUN >============
// addr R
//
char* exec_command(char* buffer) {
    static uint32_t startaddr = 0;
    char* p = buffer;
    
    while (is_address_char(*p++));
    const char funcchar = *(p-1); 
    const char* endstr = p;  
    hextou32(buffer, (p - buffer) - 1, &startaddr);
    
    while (is_address_char(*p++));
    uint32_t endaddr = 0;
    hextou32(endstr, (p - endstr) - 1, &endaddr);
    
    print_uint32(startaddr); putc(':');
    
    if (*endstr == 'R') {
        putc(CC_LINEFEED);
        exec_run(startaddr);
        *p = '\0'; // force end of a command
    }
    else if (funcchar == ':') {
        endaddr = exec_write((uint8_t*)startaddr, endstr);
        exec_read((uint8_t*)startaddr, (uint8_t*)endaddr); // confirm written data
        *p = '\0'; // force end of a command
    }
    else {
        exec_read((uint8_t*)startaddr, (uint8_t*)endaddr);
    }

    putc(CC_LINEFEED);
    return p;
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
            char* p = buffer;
            while(*(p = exec_command(p)));
            buffer[buffer_size = 0] = '\0'; // "free" buffer
            puts(promptstr);
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