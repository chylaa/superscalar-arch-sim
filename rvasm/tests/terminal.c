#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/strings.h"
#include "../simlib/io.h"

#define RX_BUFFER_SIZE 1024
#define TERMINAL_MAX_CHAR 0x7F

typedef enum { 
    CC_BACKSPACE = 0x08,
    CC_RETURN = '\r',
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

void exec_command(const char* buffer, int size) {
    puts(itoa(size));
    puts(" characters in buffer\n");
    puts(" - but no commands implemented yet :c\n");
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
        if (c == CC_BACKSPACE) {
            if (buffer_size > 0) 
                --buffer_size;
            continue;
        }
        if (c == CC_LINEFEED) {
            buffer[buffer_size] = '\0';
            exec_command(buffer, buffer_size);
            buffer[buffer_size = 0] = '\0'; // "free" buffer
            continue;
        }
        if (c == CC_RETURN) {
            buffer[buffer_size = 0] = '\0';
            continue;
        }
        // for now ignore rest of control characters
        if (c <= CC_MAX) { 
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