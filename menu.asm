NAME "LEARN"

ORG 100h

JMP MAIN;

MENU:
    DB " ----- LEARN MENU -----"
    DB 13, 10 
    DB 13, 10
    DB " 1: HELLO WORLD"
    DB 13, 10
    DB " 2: INPUT CHAR"
    DB 13, 10
    DB " 3: ECHO CHAR"
    DB 13, 10
    DB " 4: The character comes first"
    DB 13, 10
    DB " 5: The character that comes next"
    
    DB 13, 10, 13, 10, ' [1-5]: $'

MAIN:	
    MOV DX, MENU;
	MOV AH, 09H;
	INT 	21H;
	MOV	AH,	0                                                              
	INT		16H;
	JMP HelloWorld;
RET

; Ham Xuat Hello World

HELLOWORLDTEXT: DB 10, 13, "HELLO WORLD"
HelloWorld PROC
    MOV DX, HELLOWORLDTEXT
    MOV AH, 09H
    INT     21H
    MOV AH, 0
    INT     16H;    
RET

; 13 (0DH): Ve Dau Dong
; 10 (0AH): Xuong Dong
