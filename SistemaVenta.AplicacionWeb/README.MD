
## Notas
* https://localhost:7055/MotorReservas/Index?establecimientoid=3

## Pendientes
* Agregar a la base de datos la ciudad de ubicacion del establecimiento para mostrarlo en la vista Rooms
  * Si el motor se va comercializar en otros paises agregar pais
  * Falta definir la hora de Check-in y Check-out permitidas del establecimiento
  * Falta agregar a la consulta de Rooms la tabla de Categorias: Tipos de cama en cada habitacion (sencilla, doble...) Tabla Categoria

* En la DB tabla Rooms falta agregar:
  * Agregar campo titulo para mostrar en cada item del la grilla de habitaciones disponibles
  * Metros cuadrados (PND)

* En la BD tabla Establecimiento falta agregar:
  * Icono del establecimiento
  * Agregar Tabla 1 a muchos galeria de imagenes
	
* En la BD tabla Room falta agregar:
  * Agregar tabla 1 a muchos galeria de image

- Agregar validacion si no existe el establecimiento o retornar a Acceso/Login
- Validar cuantas camas y cuartos disponibles existen, mostrar un mensaje o devolver al home
- Falta agregar un loader antes y despues de hacer la reserva