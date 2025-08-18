#include "../simlib/stdio.h"

#define SLIDER_MIN 0x00
#define SLIDER_MAX 0x4A
#define SLIDER_INC 'd'
#define SLIDER_DEC 'a'
#define SLIDER_ACK '\n'
#define SLIDER_CHR '='

const char* const HEXCHARS = "0123456789ABCDEF";
uint8_t slider_value = 0;

void print_slider_value() {
    putc('\r');
    int i = 0;
    for (; i < slider_value; ++i) {
        putc(SLIDER_CHR);
    }
    for (; i < SLIDER_MAX; ++i) {
        putc(' ');
    }
    putc(' '); putc(':');
    putc(HEXCHARS[slider_value >> 4 & 0xF]);
    putc(HEXCHARS[slider_value & 0xF]);
}

int main(void) {
    io_init();
    puts("Use 'a'/'d' to control the slider. Confirm with ENTER.\n\n");
    
    int c;
    while ((c = getc()) != SLIDER_ACK) {
        if (c == SLIDER_DEC && slider_value > SLIDER_MIN) {
             --slider_value;
             print_slider_value();
        }
        else if (c == SLIDER_INC && slider_value < SLIDER_MAX) { 
            ++slider_value;
            print_slider_value();
        }
    }
    return slider_value;
}