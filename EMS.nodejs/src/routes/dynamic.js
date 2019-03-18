const express = require('express');
const router = express.Router();
const fs = require('fs');
const {promisify} = require('util');
const researchService = require('../services/research');

const RESEARCHES = {
    'Болезни лесных насаждений': 'FOREST_DISEASES',
    'Эрозия почвы': 'SOIL_EROSION',
    'Загрязнение почвы нефтепродуктами': 'SOIL_POLLUTION_BY_OIL_PRODUCTS',
    'Поверхностные свалки': 'SURFACE_DUMPS',
};

router.get('/:id', (req, res) => {
    res.render('dynamic',{requestId: req.params.id});
});

router.post('/createCharacteristics', (req, res) => {
    const research = RESEARCHES[req.body.research];
    const coord = [req.body.rightLowerLatitude, req.body.leftUpperLongtitude, req.body.leftUpperLatitude, req.body.rightLowerLongtitude];
    const requestId = req.body.request;

    try{
        req.result = researchService.determineCharacteristics(requestId, coord, research);
    }catch (e) {
        res.error('Запрос не создан, произошла ошибка');
        return res.redirect('/map');
    }

    res.info('Запрос успешно создан и принят на обработку');
    res.redirect(`/dynamic/${requestId}`);
});


module.exports = router;