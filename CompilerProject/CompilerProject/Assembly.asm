.386
.model flat, stdcall


STD_OUTPUT_HANDLE EQU -11
GetStdHandle PROTO, nStdHandle: DWORD
WriteConsole EQU <WriteConsoleA> ;alias
WriteConsole PROTO, handle: DWORD, lpBuffer:PTR BYTE, nNumberOfBytesToWrite:DWORD, lpNumberOfBytesWritten:PTR DWORD, lpReserved:DWORD



STD_INPUT_HANDLE EQU -10
ReadConsole EQU <ReadConsoleA> ;alias
ReadConsole PROTO, handle:DWORD, lpBuffer:PTR BYTE, nNumberOfCharsToRead:DWORD, lpNumberOfCharsRead:PTR DWORD, lpReserved:PTR DWORD

.data
Lit3 dd 3
Lit0 dd 0

buffer db "aaaaaaaaaa"
newline db 10,13
ReadIntegerMessage db "Enter a valid 64bit integer: "
ReadIntegerMessageLen dd offset ReadIntegerMessageLen-offset ReadIntegerMessage
;buffer BYTE 12 DUP(?),0,0
ReadInt dd 1

.data?
X dd ?
consoleOutHandle dd ?
consoleInHandle DWORD ?
bytesWritten dd ?
bytesRead DWORD ?


.code
main proc
mov eax, [Lit3]
mov [X], eax
mov eax, [X]
cmp eax, [Lit4]
JGE L1
mov eax, [X]
cmp eax, [Lit3]
JG L2
W3: 
mov eax, [X]
cmp eax, [Lit1]
JLE L3
W4: 
mov eax, [X]
cmp eax, [Lit2]
JL L4
mov eax, [X]
cmp eax, [Lit3]
JNE L5
mov eax, [Lit0]
mov [X], eax
L5: nop
mov eax, X
call ConvertIntegerToString
mov eax, offset buffer
mov ebx, 10
call PrintLine
jmp W4
L4: nop
jmp W3
L3: nop
L2: nop
L1: nop

PRINT proc
push eax
INVOKE GetStdHandle, STD_OUTPUT_HANDLE
mov consoleOutHandle, eax
pop eax
INVOKE WriteConsole, consoleOutHandle, eax, ebx, offset bytesWritten, 0

ret
PRINT endp



PrintLine proc
push eax
INVOKE GetStdHandle, STD_OUTPUT_HANDLE
mov consoleOutHandle, eax
pop eax
INVOKE WriteConsole, consoleOutHandle, eax, ebx, offset bytesWritten, 0
INVOKE WriteConsole, consoleOutHandle, offset newline, 2, offset bytesWritten, 0

ret
PrintLine endp



ReadInteger proc

mov eax, offset ReadIntegerMessage
mov ebx, ReadIntegerMessageLen
call PRINT

push STD_INPUT_HANDLE
call GetStdHandle
mov consoleInHandle, eax
INVOKE ReadConsole, consoleInHandle, offset buffer, 10, offset bytesRead, 0

call ConvertStringToInteger  ;result in eax
ret
ReadInteger endp



;
; Converts an integer to a string for printing
;
ConvertIntegerToString proc

mov ebx, offset buffer + 9

ConvertLoop:
mov edx, 0
mov ecx, 10
div ecx
add dl, '0';
mov [ebx], dl; 
dec ebx
cmp ebx, offset buffer
jge ConvertLoop

ret
ConvertIntegerToString endp



;
; Converts a string read from the keyboard into an integer
;
ConvertStringToInteger proc
mov eax, 0
mov [ReadInt], eax
mov ecx, offset buffer

mov bx,0
mov bl, BYTE PTR [ecx]
Next:
sub bl, '0'
mov eax, [ReadInt]
mov dx, 10
mul dx
add ax, bx
mov [ReadInt], eax

mov bx, 0
add ecx, 1
mov bl, BYTE PTR [ecx]

cmp bl, 0Dh ;input is terminated with 0xD 0xA
jne Next

ret
ConvertStringToInteger endp

mov eax, 1
ret
main endp
end