#ifndef LEDMATRIX_H
#define LEDMATRIX_H

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <termios.h>
#include <sys/select.h>
#include <linux/input.h>
#include <stdbool.h>
#include <string.h>
#include <time.h>
#include <poll.h>

#include <fcntl.h>
#include <sys/ioctl.h>
#include <linux/fb.h>
#include <sys/mman.h>
#include <stdint.h>

#ifdef BUILD_DLL
#define INEX __declspec(dllexport)
#else
#define INEX __declspec(dllimport)
#endif

void set_LED_matrix_pixel(uint8_t);

#endif