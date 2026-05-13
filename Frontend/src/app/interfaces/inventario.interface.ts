export default interface Inventario {
    id?: string;
    fecha: Date;
    secuencial: number;
    observacion: string;
    usuario: string,
    fechainteger: number;
    items: [{
        id?: string;
        nombre: string,
        existencia: number,
    }]
}