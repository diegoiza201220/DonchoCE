export default interface Ordencocina{
    id?: string;
    secuencialorden: string;
    producto: string;
    cantidad: number;
    recibido: boolean;
    procesado: boolean;
    entregado: boolean;
    observacion: string;
}