.386
.model flat, stdcall


STD_OUTPUT_HANDLE EQU -11
GetStdHandle PROTO, nStdHandle: DWORD
WriteConsole EQU <WriteConsoleA> ;alias
WriteConsole PROTO, handle: DWORD, lpBuffer:PTR BYTE, nNumberOfBytesToWrite:DWORD, lpNumberOfBytesWritten:PTR DWORD, lpReserved:DWORD



STD_INPUT_HANDLE EQU -10
ReadConsole EQU <ReadConsoleA> ;alias
ReadConsole PROTO, handle:DWORD, lpBuffer:PTR BYTE, nNumberOfCharsToRead:DWORD, lpNumberOfCharsRead:PTR DWORD, lpReserved:PTR DWORD
buffer db "aaaaaaaaaa"
newline db 10,13
ReadIntegerMessage db "Enter a valid 64bit integer: "
ReadIntegerMessageLen dd offset ReadIntegerMessageLen-offset ReadIntegerMessage
;buffer BYTE 12 DUP(?),0,0
ReadInt dd 1
.data?T0 dd ?
call ReadInteger
mov X, eax
call ReadInteger
mov Y, eax
mov eax, [X]
cmp eax, [Y]
JLE L1
mov eax, X
call ConvertIntegerToString
mov eax, offset buffer
mov ebx, 10
call PrintLine
L1: nop
mov eax, [X]
cmp eax, [Y]
JGE L2
mov eax, Y
call ConvertIntegerToString
mov eax, offset buffer
mov ebx, 10
call PrintLine
L2: nop
