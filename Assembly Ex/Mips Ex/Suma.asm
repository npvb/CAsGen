      .data
privaterMem: .word   0 : 2
x: .word
      .text
  la $a0, privaterMem
  jal main 
  
main: 

	add $a0,$a0,4
	addi $t0,$zero,10
	sw $t0,($a0)
	
	add $a0,$a0,4
	addi $t0,$zero,20
	sw $t0,($a0)
	
	jal _addint_int
	
_addint_int: 
	addi $sp, $sp, -16  #suma(int a, int b) { int res;
	sw $ra, -16($sp)
	
	lw $t0, ($a0) #cargar paramatro "a" en temporal
	sw $t0, -4($sp) #assignar temporal en "a" local
	addi $a0,$a0,-4 #pop stack de parametros
	
	lw $t0, ($a0) #cargar paramatro "b" en temporal
	sw $t0, -8($sp) #assignar temporal en "b" local
	addi $a0,$a0,-4 #pop stack de parametros
	
	lw $t0,  -4($sp) #cargar local "a" a temporal
	lw $t1,  -8($sp) #cargar local "b" a temporal
	
	add $t0,$t0,$t1 #a + b //$t0 = a + b
	
	sw $t0, -12($sp) #res = $t0
	
	lw $t0, -12($sp) #cargar local "res" a temporal
	
	add $v0, $t0, $zero #return "res"
	
	lw $ra, -16($sp)
	addi $sp, $sp, 16
	jr $ra
	
_add2: 
	addi $sp, $sp, -20  #suma(int a, int b, int c) { int res;
	sw $ra, -20($sp)
	
	lw $t0, ($a0) #cargar paramatro "a" en temporal
	sw $t0, -4($sp) #assignar temporal en "a" local
	addi $a0,$a0,-4 #pop stack de parametros
	
	lw $t0, ($a0) #cargar paramatro "b" en temporal
	sw $t0, -8($sp) #assignar temporal en "b" local
	addi $a0,$a0,-4 #pop stack de parametros
	
	lw $t0, ($a0) #cargar paramatro "c" en temporal
	sw $t0, -12($sp) #assignar temporal en "c" local
	addi $a0,$a0,-4 #pop stack de parametros
	
	lw $t0,  -4($sp) #cargar local "a" a temporal
	lw $t1,  -8($sp) #cargar local "b" a temporal
	
	add $t0,$t0,$t1 #a + b //$t0 = a + b
	
	lw $t1,  -12($sp) #cargar local "c" a temporal
	
	add $t0, $t0, $t1 #(a + b) + c //$t0 = ($t0) + c
	
	sw $t0, -16($sp) #res = $t0
	
	lw $t0, -16($sp) #cargar local "res" a temporal
	
	add $v0, $t0, $zero #return "res"
	
	lw $ra, -20($sp)
	addi $sp, $sp, 20
	jr $ra
