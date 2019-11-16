import styles from '../resources/css/styles.css';

/**
 * @var {Object} elements
 */
export const elements = {
    body: document.querySelector('body')
};

/**
 * 
 * @param {number} decimal 
 */
export const formatDecimal = decimal => {
    return decimal.toFixed(2);
};

/**
 * 
 * @param {string} paymentMethod 
 */
export const formatPaymentMethod = paymentMethod => {
    return paymentMethod.replace("e", "é");
};