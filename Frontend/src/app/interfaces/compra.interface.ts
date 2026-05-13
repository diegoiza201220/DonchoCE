export default interface Compra {
    id?: string;
    fecha: Date;
    fechareal: Date;
    secuencial: number;
    numerodocumento: string;
    proveedor: string;
    total: number;
    usuario: string;
    fechainteger: number,
    items: [{
        id?: string;
        nombre: string,
        cantidad: number,
        preciounitario: string,
        preciototal: number
    }]
}