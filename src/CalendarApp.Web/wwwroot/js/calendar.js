// ===== SVG ICONS =====
const ICON = {
    calendar: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>',
    clipboard: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2"/><rect x="8" y="2" width="8" height="4" rx="1" ry="1"/></svg>',
    clock: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>',
    mapPin: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/><circle cx="12" cy="10" r="3"/></svg>',
    bell: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>',
    users: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>',
    trash: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>',
    check: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"/></svg>',
    x: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>',
    alert: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>',
    info: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>',
    refresh: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg>'
};

// ===== STATE =====
let pendingAppointment = null;
let pendingConflictingId = null;
let pendingGroupMeeting = null;
let selectedDetailId = null;

// ===== MODAL =====
function openModal(id) {
    const el = document.getElementById(id);
    if (!el) return;
    el.classList.add('active');
    document.body.style.overflow = 'hidden';
    setTimeout(() => {
        const input = el.querySelector('input[type="text"], input[type="date"]');
        if (input) input.focus();
    }, 100);
}

function closeModal(id) {
    const el = document.getElementById(id);
    if (!el) return;
    el.classList.remove('active');
    document.body.style.overflow = '';
}

// Close modal on overlay click
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-overlay') && e.target.classList.contains('active')) {
        e.target.classList.remove('active');
        document.body.style.overflow = '';
    }
});

// Close modal on Escape
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-overlay.active').forEach(m => m.classList.remove('active'));
        document.body.style.overflow = '';
    }
});

// ===== TOAST =====
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    if (!container) return;
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    const icons = { success: ICON.check, error: ICON.x, warning: ICON.alert, info: ICON.info };
    toast.innerHTML = `<span class="toast-icon">${icons[type] || icons.check}</span><span>${message}</span>`;
    container.appendChild(toast);
    toast.addEventListener('click', () => removeToast(toast));
    setTimeout(() => removeToast(toast), 4000);
}

function removeToast(toast) {
    if (toast.classList.contains('removing')) return;
    toast.classList.add('removing');
    setTimeout(() => toast.remove(), 300);
}

// ===== HELPERS =====
/**
 * Parse an ISO datetime string and return "HH:mm" without timezone conversion.
 * e.g. "2026-05-07T09:30:00" → "09:30"
 */
function formatTime(isoString) {
    if (!isoString) return '--:--';
    const timePart = isoString.includes('T') ? isoString.split('T')[1] : isoString;
    const parts = timePart.split(':');
    return `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`;
}

function showFieldError(elementId, message) {
    const el = document.getElementById(elementId);
    if (!el) return;
    const span = el.querySelector('span:last-child');
    if (span) span.textContent = message;
    el.classList.add('visible');
    if (el.previousElementSibling) el.previousElementSibling.classList.add('error');
}

function clearErrors() {
    document.querySelectorAll('.error-text').forEach(el => el.classList.remove('visible'));
    document.querySelectorAll('.form-input.error').forEach(el => el.classList.remove('error'));
}

// ===== FORM HELPERS =====
function resetForm() {
    document.getElementById('apptName').value = '';
    document.getElementById('apptLocation').value = '';
    document.getElementById('apptReminder').checked = false;
    document.getElementById('apptStartTime').value = '09:00';
    document.getElementById('apptEndTime').value = '10:00';
    clearErrors();
}

// ===== OPEN ADD MODAL (FAB button) =====
function openAddModal() {
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('apptDate').value = today;
    resetForm();
    openModal('addModal');
}

// ===== OPEN ADD FOR DATE (Calendar day click) =====
function openAddForDate(date) {
    document.getElementById('apptDate').value = date;
    resetForm();
    openModal('addModal');
}

// ===== REOPEN FORM FOR "CHOOSE OTHER TIME" (Sequence Diagram Option 1) =====
// Keeps name, location, date — user only needs to pick a new time
function reopenFormForOtherTime() {
    if (!pendingAppointment) return;
    document.getElementById('apptName').value = pendingAppointment.name || '';
    document.getElementById('apptLocation').value = pendingAppointment.location || '';
    document.getElementById('apptReminder').checked = pendingAppointment.addReminder || false;

    // Restore the same date from pending data
    if (pendingAppointment.startTime) {
        const datePart = pendingAppointment.startTime.split('T')[0];
        document.getElementById('apptDate').value = datePart;
    }

    // Reset time so user picks a fresh slot
    document.getElementById('apptStartTime').value = '09:00';
    document.getElementById('apptEndTime').value = '10:00';
    clearErrors();

    // Clear conflict state
    pendingAppointment = null;
    pendingConflictingId = null;

    openModal('addModal');
}

// ===== SUBMIT APPOINTMENT =====
async function submitAppointment(event) {
    if (event) event.preventDefault();
    clearErrors();

    const name = document.getElementById('apptName').value.trim();
    const location = document.getElementById('apptLocation').value.trim();
    const date = document.getElementById('apptDate').value;
    const addReminder = document.getElementById('apptReminder').checked;
    const startTime = document.getElementById('apptStartTime').value; // "HH:mm"
    const endTime = document.getElementById('apptEndTime').value;     // "HH:mm"

    let hasError = false;

    if (!name) {
        showFieldError('nameError', 'Name is required');
        hasError = true;
    }

    if (!date) {
        showToast('Please select a date', 'error');
        hasError = true;
    }

    if (!startTime || !endTime) {
        showFieldError('timeError', 'Please select start and end time');
        hasError = true;
    } else if (endTime <= startTime) {
        showFieldError('timeError', 'End time must be after start time');
        hasError = true;
    }

    if (hasError) return;

    const cmd = {
        name,
        location,
        startTime: `${date}T${startTime}:00`,
        endTime: `${date}T${endTime}:00`,
        addReminder,
        userId: 1
    };

    const btn = document.getElementById('btnSave');
    const originalText = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = `<div class="spinner"></div> Saving...`;

    try {
        const response = await fetch('/api/AppointmentApi', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cmd)
        });

        const result = await response.json();

        if (response.status === 400) {
            showFieldError('nameError', result.error || 'Validation failed');
            showToast(result.error || 'Validation failed', 'error');

        } else if (response.status === 409) {
            // Conflict found → show conflict modal
            closeModal('addModal');
            pendingAppointment = cmd;
            pendingConflictingId = result.conflictingAppointment?.id;
            showConflictDialog(result.conflictingAppointment);

        } else if (result.action === 'join_group') {
            // Group meeting match found
            closeModal('addModal');
            pendingAppointment = cmd;
            pendingGroupMeeting = result.groupMeeting;
            showJoinGroupDialog(result.groupMeeting);

        } else if (response.ok) {
            closeModal('addModal');
            showToast('Appointment saved!', 'success');
            refreshCalendar();
        }
    } catch (err) {
        showToast('Network error. Please try again.', 'error');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalText;
    }
}

// ===== CONFLICT DIALOG =====
function showConflictDialog(conflicting) {
    const startStr = formatTime(conflicting.startTime);
    const endStr = formatTime(conflicting.endTime);
    document.getElementById('conflictMessage').innerHTML =
        `You already have <strong>"${conflicting.name}"</strong> scheduled from <strong>${startStr}</strong> to <strong>${endStr}</strong>.`;
    openModal('conflictModal');
}

async function resolveConflict(option) {
    if (!pendingAppointment) return;
    closeModal('conflictModal');

    // Option 1: Choose Other Time → reopen form keeping same name/date
    if (option === 'choose_other_time') {
        reopenFormForOtherTime();
        return;
    }

    // Option 2: Replace Existing → delete old, then check group meeting, then save new
    if (option === 'replace') {
        try {
            const resp = await fetch('/api/AppointmentApi/resolve-conflict', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    command: pendingAppointment,
                    conflictingAppointmentId: pendingConflictingId,
                    option: 1  // ConflictOption.ReplaceExisting
                })
            });

            const result = await resp.json();

            if (result.action === 'join_group') {
                // Backend found a matching group meeting → show join dialog
                // pendingAppointment is still set, just update pendingGroupMeeting
                pendingGroupMeeting = result.groupMeeting;
                pendingConflictingId = null;
                showJoinGroupDialog(result.groupMeeting);
            } else if (resp.ok) {
                showToast('Appointment replaced successfully!', 'success');
                refreshCalendar();
                pendingAppointment = null;
                pendingConflictingId = null;
            } else {
                showToast('Failed to replace appointment', 'error');
                pendingAppointment = null;
                pendingConflictingId = null;
            }
        } catch (err) {
            showToast('Network error', 'error');
            pendingAppointment = null;
            pendingConflictingId = null;
        }
    }
}

function cancelConflict() {
    closeModal('conflictModal');
    pendingAppointment = null;
    pendingConflictingId = null;
    showToast('Cancelled', 'info');
}

// ===== GROUP MEETING DIALOG =====
function showJoinGroupDialog(group) {
    document.getElementById('groupMessage').innerHTML =
        `Found group meeting <strong>"${group.name}"</strong> with <strong>${group.participantCount}</strong> participant(s) (same name & duration).`;
    openModal('groupModal');
}

async function confirmJoinGroup() {
    if (!pendingAppointment || !pendingGroupMeeting) return;
    closeModal('groupModal');

    try {
        const resp = await fetch('/api/AppointmentApi/join-group', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                groupMeetingId: pendingGroupMeeting.id,
                userId: 1,
                appointmentStartTime: pendingAppointment.startTime,
                appointmentEndTime: pendingAppointment.endTime,
                appointmentName: pendingAppointment.name,
                appointmentLocation: pendingAppointment.location
            })
        });
        if (resp.ok) {
            showToast('Joined group meeting!', 'success');
            refreshCalendar();
        } else {
            showToast('Failed to join group meeting', 'error');
        }
    } catch (err) {
        showToast('Network error', 'error');
    }

    pendingAppointment = null;
    pendingGroupMeeting = null;
}

// Skip group meeting → save as personal appointment directly
async function skipGroup() {
    closeModal('groupModal');
    if (!pendingAppointment) return;

    try {
        const resp = await fetch('/api/AppointmentApi/save-personal', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(pendingAppointment)
        });
        if (resp.ok) {
            showToast('Personal appointment created!', 'success');
            refreshCalendar();
        } else {
            showToast('Failed to save appointment', 'error');
        }
    } catch (err) {
        showToast('Network error', 'error');
    }

    pendingAppointment = null;
    pendingGroupMeeting = null;
}

// ===== DETAIL MODAL =====
function showDetail(id, name, startTime, endTime, location, hasReminder, isGroup, event) {
    if (event) event.stopPropagation();
    selectedDetailId = id;

    document.getElementById('detailTitle').innerHTML = `${ICON.clipboard} ${name}`;
    document.getElementById('detailBody').innerHTML = `
        <div class="detail-row">
            <div class="detail-icon time">${ICON.clock}</div>
            <div><div class="detail-label">Time</div><div class="detail-value">${startTime} – ${endTime}</div></div>
        </div>
        ${location ? `<div class="detail-row">
            <div class="detail-icon location">${ICON.mapPin}</div>
            <div><div class="detail-label">Location</div><div class="detail-value">${location}</div></div>
        </div>` : ''}
        <div class="detail-row">
            <div class="detail-icon reminder">${ICON.bell}</div>
            <div><div class="detail-label">Reminder</div><div class="detail-value">${hasReminder ? '15 min before' : 'None'}</div></div>
        </div>
        ${isGroup ? `<div class="detail-row">
            <div class="detail-icon group">${ICON.users}</div>
            <div><div class="detail-label">Type</div><div class="detail-value">Group Meeting</div></div>
        </div>` : ''}
    `;
    openModal('detailModal');
}

async function deleteAppointment() {
    if (!selectedDetailId) return;
    if (!confirm('Delete this appointment?')) return;

    try {
        const resp = await fetch(`/api/AppointmentApi/${selectedDetailId}`, { method: 'DELETE' });
        if (resp.ok) {
            closeModal('detailModal');
            showToast('Deleted!', 'success');
            refreshCalendar();
        } else {
            showToast('Failed to delete', 'error');
        }
    } catch (err) {
        showToast('Network error', 'error');
    }
}

// ===== REFRESH CALENDAR =====
async function refreshCalendar() {
    try {
        const resp = await fetch(window.location.href);
        const html = await resp.text();
        const doc = new DOMParser().parseFromString(html, 'text/html');

        const newGrid = doc.querySelector('.calendar-grid');
        const currentGrid = document.querySelector('.calendar-grid');
        if (newGrid && currentGrid) currentGrid.innerHTML = newGrid.innerHTML;

        const newSidebar = doc.querySelector('.sidebar-panel');
        const currentSidebar = document.querySelector('.sidebar-panel');
        if (newSidebar && currentSidebar) currentSidebar.innerHTML = newSidebar.innerHTML;
    } catch (err) {
        window.location.reload();
    }
}

// ===== REMINDER POLLING =====
async function checkReminders() {
    try {
        const resp = await fetch('/api/AppointmentApi/reminders');
        if (!resp.ok) return;
        const reminders = await resp.json();
        if (reminders.length > 0) {
            const r = reminders[0];
            document.getElementById('reminderApptName').textContent = r.name;
            openModal('reminderModal');
            try { new Audio('https://assets.mixkit.co/active_storage/sfx/2869/2869-preview.mp3').play(); } catch (e) { }
        }
    } catch (err) {
        console.error('Reminder check failed', err);
    }
}

// ===== INIT =====
document.addEventListener('DOMContentLoaded', function () {
    // Start reminder polling every 30 seconds
    setInterval(checkReminders, 30000);
    checkReminders();
});
