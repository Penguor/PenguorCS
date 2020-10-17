; tell assembler to generate 64-bit code
bits 64

; data segment
section .data use64

    message..45 db "hello!"     ; friendly greeting

; set up the .text segment for the code
section .text use64

; global main is the entry point
global main

extern printf

Hello.main:
    mov rcx, message..45
    call printf

    mov rsp, rbp
    ret

main:
    jmp Hello.main