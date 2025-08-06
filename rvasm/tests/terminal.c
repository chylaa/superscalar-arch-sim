#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/strings.h"

#define IS_BIT_SET(value, bit) (((value) & (1 << bit)) >> bit)
#define SET_BIT(value, bit) (value = ((value) | (1 << bit)))
#define RESET_BIT(value, bit) (value = ((value) & ~(1 << bit)))

volatile uint8_t* const IO_CONTROL = (uint8_t* const)0x008F0000;   
volatile const uint8_t* const IO_RX_BUFFER = (const uint8_t* const)0x008F0001;   
volatile uint8_t* const IO_TX_BUFFER = (uint8_t* const)0x008F0002;   

// '0' we fetched character from IO_RX_BUFFER, set to '1' by terminal upon sending us a character
#define IO_CONTROL_RX_BIT 0
// '1' we put character in IO_TX_BUFFER, set to '0' by terminal as ackowledge of receiving
#define IO_CONTROL_TX_BIT 1

#define IO_IS_RX_AVALIABLE() IS_BIT_SET(*IO_CONTROL, IO_CONTROL_RX_BIT)
#define IO_IS_TX_READY() (!IS_BIT_SET(*IO_CONTROL, IO_CONTROL_TX_BIT))

#define IO_SINGAL_TX() SET_BIT(*IO_CONTROL, IO_CONTROL_TX_BIT)
#define IO_SIGNAL_RX() RESET_BIT(*IO_CONTROL, IO_CONTROL_RX_BIT) 

#define IO_ERROR (-1)

#define RX_BUFFER_SIZE 1024

typedef enum { 
    CC_BACKSPACE = 0x08,
    CC_RETURN = '\r',
    CC_LINEFEED = '\n',
    CC_MAX = 0x1F
} ControlChars;

static int cwrite(char c) {
    if (IO_IS_TX_READY()) {
        *IO_TX_BUFFER = c;
        IO_SINGAL_TX();
        return c;
    }
    return IO_ERROR;
}
static int cread() {
    if(IO_IS_RX_AVALIABLE()) {
        uint8_t c = *IO_RX_BUFFER;
        IO_SIGNAL_RX();
        return c;
    }
    return IO_ERROR;
}

static int putc(char c) {
    while (cwrite(c) == IO_ERROR);
    return c; 
}

static int getc() {
    int c;
    while ((c = cread()) == IO_ERROR); 
    return c;
}

static int puts(const char* s) {
    int i; char c;
    for (i = 0; c = s[i]; ++i)
        putc(c);
    return i;
}

static void term_init() {
    RESET_BIT(*IO_CONTROL, IO_CONTROL_TX_BIT); // force nothing in TX buffer
    IO_SIGNAL_RX(); // we're ready to receive
}

static int get_echo_char() {
    int received = getc();
    int isascii = received < 0x80;
    if (isascii) {
        return putc(received);
    }
    return IO_ERROR;
}

static const char* promptstr = "> ";
static char cbuffer[RX_BUFFER_SIZE] = {0};
static char* bufferptr = cbuffer;

void exec_command(const char* buffer, int size) {
    puts(itoa(size));
    puts(" characters in buffer\n");
    puts(" - but no commands implemented yet :c\n");
    puts(promptstr);
}

int main(void) {
    term_init();
    puts(promptstr);
    
    char* buffer = cbuffer;
    int buffer_size = 0;
    int buffer_max_size = RX_BUFFER_SIZE;
    
    for(;;) {
        int c = get_echo_char();
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