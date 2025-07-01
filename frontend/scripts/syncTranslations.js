import fs from 'fs';
import path from 'path';
import translatte from 'translatte';

const localesDir = path.join(path.dirname(new URL(import.meta.url).pathname), '../i18n/locales');
const languages = ['en', 'es', 'de', 'ja', 'zh'];
const referenceFile = path.join(localesDir, 'fr.json');
const referenceTranslations = JSON.parse(fs.readFileSync(referenceFile, 'utf8'));

const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));

const translateKey = async (text, targetLang) => {
  try {
    const res = await translatte(text, { to: targetLang });
    return res.text;
  } catch (err) {
    console.error(`Erreur de traduction pour "${text}" en ${targetLang}:`, err);
    return '';
  }
};

const syncTranslations = async () => {
  for (const lang of languages) {
    const filePath = path.join(localesDir, `${lang}.json`);
    let translations = {};

    if (fs.existsSync(filePath)) {
      translations = JSON.parse(fs.readFileSync(filePath, 'utf8'));
    }

    for (const key of Object.keys(referenceTranslations)) {
      if (!(key in translations)) {
        const textToTranslate = referenceTranslations[key];
        if (lang === 'fr') {
          translations[key] = textToTranslate;
        } else {
          await delay(500); // attendre entre les requêtes pour éviter le throttle
          translations[key] = await translateKey(textToTranslate, lang);
          console.log(`[${lang}] ${key}: ${translations[key]}`);
        }
      }
    }

    fs.writeFileSync(filePath, JSON.stringify(translations, null, 2), 'utf8');
    console.log(`Fichier synchronisé : ${lang}.json`);
  }
};

syncTranslations();
