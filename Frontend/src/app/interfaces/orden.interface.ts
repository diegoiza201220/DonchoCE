export default interface Orden {
    id?: string;
    fecha: Date;
    particion: number;
    secuencial: number;
    cliente: string;
    ruc: string;
    direccion: string;
    email: string;
    telefono: string;
    tipodepago: string;
    documentopago: string;
    totalorden: number,
    usuario: string,
    productos: [{
        cantidad: number,
        plato: string,
        preciounitario: number,
        preciototal: number,
        pedidoacocina: boolean
    }]
}