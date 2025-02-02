export type Reminder = {

    id: string,
    createdBy: string,
    content: string,
    color: string,
    coordX: number,
    coordY: number,
    coordZ: number
};

export type Locale = 'fr' | 'en' | 'es' | 'de' | 'zh' | 'ja';
