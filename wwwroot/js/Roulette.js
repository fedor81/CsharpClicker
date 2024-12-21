const canvas = document.querySelector('canvas');
const context = canvas.getContext('2d');
const rollButton = document.getElementById("roulette-roll-button");
const score = document.getElementById("current-score");
const betInput = document.getElementById("bet-input");

const IMAGE_WIDTH = 128;
const IMAGE_HEIGHT = 128;
const IMAGE_COUNT = 3;
const OFFSET = 1;
const BASE_SPEED = 5;
const ACCELERATION_DURATION_MIN = 500;
const ACCELERATION_DURATION_MAX = 1500;
const ACCELERATION_STEP = 1;
const DECELERATION_MULTIPLIER = 0.95;
const RETURN_MULTIPLIER = 0.1;
const STATE = {
    ACCELERATION: 1,
    DECELERATION: 2,
    RETURN: 3
};

const images = [];
const imageUrls = [
    'http://localhost:5019/img/roulette-icons/green_numbers/0.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/1.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/2.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/3.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/4.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/5.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/6.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/7.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/8.png',
    'http://localhost:5019/img/roulette-icons/green_numbers/9.png',
];
let speed = 0;
let state = STATE.RETURN;
let startIndex = 0;
let startTime = 0;
let accelerationDuration = 0;
let offset = 0;

let rouletteSet;
let target = 0;
let targetIndex = 0;
let currentScore = Number(score.innerText);
let minBet = Number(betInput.min);


document.getElementById("bet-min-button").onclick = () => {
    betInput.value = minBet;
}

document.getElementById("bet-max-button").onclick = () => {
    betInput.value = Math.max(minBet, currentScore);
}


// Перетасовка списка
function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
        let j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
}

const loadImage = (url) => fetch(url)
    .then(response => response.blob())
    .then(createImageBitmap);

const random = (min, max) => Math.floor(Math.random() * (max - min + 1) + min);

function FindTargetIndex(target, currentIndex) {
    for (let i = 0; i < rouletteSet.length; i++) {
        currentIndex = (currentIndex + 1) % rouletteSet.length;
        if (rouletteSet[currentIndex] === target) {
            return currentIndex;
        }
    }
}

const draw = () => {
    const imagesLength = images.length;
    const center = Math.floor(canvas.width / 2)

    context.fillStyle = '#ffffff';
    context.fillRect(0, 0, canvas.width, canvas.height);

    for (let index = -OFFSET; index < IMAGE_COUNT + OFFSET; index++) {
        const rouletteIndex = rouletteSet[(index + startIndex + rouletteSet.length) % rouletteSet.length];
        const image = images[rouletteIndex % images.length];

        context.drawImage(
            image,
            IMAGE_WIDTH * index - offset,
            0,
            IMAGE_WIDTH,
            IMAGE_HEIGHT
        );
    }

    context.moveTo(center + 0.5, 0);
    context.lineTo(center + 0.5, canvas.height);
    context.closePath();
    context.strokeStyle = 'rgba(0, 0, 0, 0.5)';
    context.stroke();
};

const update = () => {
    const imagesLength = images.length;
    const deltaTime = performance.now() - startTime;

    if (deltaTime > accelerationDuration && state === STATE.ACCELERATION) {
        state = STATE.DECELERATION;
    }

    if (offset > IMAGE_WIDTH) {
        startIndex = (startIndex + 1) % imagesLength;
        offset %= IMAGE_WIDTH;
    }

    draw();

    const center = IMAGE_WIDTH * IMAGE_COUNT / 2;
    const index = Math.floor((center + offset) / IMAGE_WIDTH);

    offset += speed;
    if (state === STATE.ACCELERATION) {
        speed += ACCELERATION_STEP;
    } else if (state === STATE.DECELERATION) {
        //if (targetIndex === null) {
        //    targetIndex = FindTargetIndex(target, rouletteIndex);
        //}

        //distanceToTarget = targetIndex > index ? targetIndex - rouletteIndex : rouletteSet.length - rouletteIndex + targetIndex;
        speed *= DECELERATION_MULTIPLIER;
        //speed -= Math.pow(distanceToTarget, 2) * 0.001;

        if (speed < 1e-2) {
            speed = 0;
            state = STATE.RETURN;
        }
    } else if (state === STATE.RETURN) {
        const halfCount = Math.floor(IMAGE_COUNT / 2);
        const distance = IMAGE_WIDTH * (index - halfCount) - offset;
        const step = distance * RETURN_MULTIPLIER;

        offset += Math.max(0.1, Math.abs(step)) * Math.sign(step);

        if (Math.abs(offset) <= 0.1) {
            offset = 0;
        }
    }

    if (speed > 0 || offset !== 0) {
        requestAnimationFrame(update);
    } else {
        const winner = (index + startIndex) % imagesLength;

        context.fillStyle = 'rgba(255, 0, 255, 0.2)';
        context.fillRect(index * IMAGE_WIDTH - offset, 0, IMAGE_WIDTH, IMAGE_HEIGHT);
        score.innerText = currentScore;

        console.group('Winner');
        console.log('Index', winner);
        console.log('Image', imageUrls[winner]);
        console.groupEnd();
    }
};


function makeBet() {
    let betNumber = Number(betInput.value);

    $.ajax({
        url: '/roulette',
        method: 'post',
        dataType: 'json',
        data: { bet: betNumber },
        success: (response) => onMakeBetSuccess(response),
    });
}

function onMakeBetSuccess(response) {
    currentScore = Number(response['currentScore']);
    betInput.max = Math.max(minBet, currentScore);
    shuffle(rouletteSet);
    rollRulette(response['winnerIndex']);
}


function rollRulette(target) {
    startTime = performance.now();
    accelerationDuration = random(ACCELERATION_DURATION_MIN, ACCELERATION_DURATION_MAX);
    state = STATE.ACCELERATION;
    speed = BASE_SPEED;
    targetIndex = null;
    target = target;

    requestAnimationFrame(update);
}

// Получение сета рулетки
function loadRoletteSet() {
    $.ajax({
        url: '/roulette/set',
        success: function (data) {
            rouletteSet = data;
            shuffle(rouletteSet);
        }
    });
}


const init = async () => {
    [canvas.width, canvas.height] = [IMAGE_WIDTH * IMAGE_COUNT, IMAGE_HEIGHT];
    loadRoletteSet();

    console.group('Loading images');
    for (const imageUrl of imageUrls) {
        console.group(imageUrl);
        console.time('loading');
        images.push(await loadImage(imageUrl));
        console.timeEnd('loading');
        console.groupEnd();
    }
    console.log(images);
    console.groupEnd();

    rollButton.addEventListener('click', event => {
        event.preventDefault();

        if (speed === 0 && offset === 0 && currentScore >= minBet) {
            makeBet();
        }
    });

    draw();
};

window.addEventListener('DOMContentLoaded', init);
