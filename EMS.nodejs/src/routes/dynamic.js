const express = require('express');
const router = express.Router();
const {Users, Requests} = require('../db/index');
const boom = require('boom');
const {validate} = require('../../utils');
const createResearch = require('../schemes/createResearch');
const fs = require('fs');
const {promisify} = require('util');
const exists = promisify(fs.exists);
const readFile = promisify(fs.readFile);

router.get('/:id', (req, res) => {
    res.render('dynamic',{requestId: req.params.id});
});


module.exports = router;